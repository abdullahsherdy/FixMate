# Use the official .NET SDK image as the base image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution and project files
COPY ["FixMate.API/FixMate.API.csproj", "FixMate.API/"]
COPY ["FixMate.Application/FixMate.Application.csproj", "FixMate.Application/"]
COPY ["FixMate.Domain/FixMate.Domain.csproj", "FixMate.Domain/"]
COPY ["FixMate.Infrastructure/FixMate.Infrastructure.csproj", "FixMate.Infrastructure/"]

# Restore NuGet packages
RUN dotnet restore "FixMate.API/FixMate.API.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
RUN dotnet build "FixMate.API/FixMate.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "FixMate.API/FixMate.API.csproj" -c Release -o /app/publish

# Use the official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port 80
EXPOSE 80

# Start the application
ENTRYPOINT ["dotnet", "FixMate.API.dll"] 