using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using FluentValidation;

using FixMate.Infrastructure.Persistence;
using FixMate.Application.Configuration;
using FixMate.Application.Interfaces.Services;
using FixMate.Application.Services;
using FixMate.Application.Interfaces.Persistence;
using FixMate.Infrastructure.Persistence.Repositories;
using FixMate.Application.DTOs;
using FixMate.Application.Validators;
using FixMate.Infrastructure.Persistence.Seeders;
using FixMate.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog Logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();

// Load and bind AppSettings
var appSettings = builder.Configuration.Get<AppSettings>();
builder.Services.Configure<AppSettings>(builder.Configuration);

// Add DbContext
builder.Services.AddDbContext<FixMateDbContext>(options =>
    options.UseSqlServer(appSettings.ConnectionStrings.DefaultConnection));

// Add Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = appSettings.Jwt.Issuer,
            ValidAudience = appSettings.Jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Jwt.Key))
        };
    });

// Add Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(appSettings.Cors.AllowedOrigins.ToArray())
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Controllers
builder.Services.AddControllers();

// Add Swagger (with JWT auth)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FixMate API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// Register Application Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();
builder.Services.AddScoped<IServiceProviderService, ServiceProviderService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();

// Register Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
builder.Services.AddScoped<IServiceProviderRepository, ServiceProviderRepository>();

// Register Validators
builder.Services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
builder.Services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();
builder.Services.AddScoped<IValidator<ChangePasswordRequest>, ChangePasswordRequestValidator>();
builder.Services.AddScoped<IValidator<UserDto>, UserDtoValidator>();
builder.Services.AddScoped<IValidator<VehicleDto>, VehicleDtoValidator>();
builder.Services.AddScoped<IValidator<CreateVehicleDto>, CreateVehicleDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateVehicleDto>, UpdateVehicleDtoValidator>();
builder.Services.AddScoped<IValidator<ServiceRequestDto>, ServiceRequestDtoValidator>();
builder.Services.AddScoped<IValidator<CreateServiceRequestDto>, CreateServiceRequestDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateServiceRequestDto>, UpdateServiceRequestDtoValidator>();
builder.Services.AddScoped<IValidator<AssignServiceProviderDto>, AssignServiceProviderDtoValidator>();
builder.Services.AddScoped<IValidator<ServiceProviderDto>, ServiceProviderDtoValidator>();
builder.Services.AddScoped<IValidator<CreateServiceProviderDto>, CreateServiceProviderDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateServiceProviderDto>, UpdateServiceProviderDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateAvailabilityDto>, UpdateAvailabilityDtoValidator>();


// Add database seeder
builder.Services.AddScoped<DatabaseSeeder>();

var app = builder.Build();

// Run Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware pipeline
app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<GlobalExceptionHandler>();

// Apply migrations and seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<FixMateDbContext>();
        context.Database.Migrate();

        var seeder = services.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during DB migration or seeding.");
    }
}

app.Run();
