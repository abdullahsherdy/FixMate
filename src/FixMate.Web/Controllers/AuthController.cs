using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using FixMate.Application.DTOs;
using FixMate.Application.Interfaces.Services;
using FixMate.Application.Configuration;
using Microsoft.Extensions.Logging;

namespace FixMate.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IServiceProviderService _serviceProvider;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IServiceProviderService serviceProvider,
            IAuthService authService,
            IJwtService jwtService,
            ILogger<AuthController> logger)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        {
            
                 
            try
            {
                var user = await _authService.ValidateUserAsync(request.Email, request.Password);
                if (user == null)
                    return Unauthorized(new { message = "Invalid email or password" });

                var token = _jwtService.GenerateToken(user);
                return Ok(new AuthResponse { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for user {Email}", request.Email);
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
        {
            try
            {
                var user = await _authService.RegisterUserAsync(request);
                if (user == null)
                    return BadRequest(new { message = "User registration failed" });

                var token = _jwtService.GenerateToken(user);
                return Ok(new AuthResponse { Token = token });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration for user {Email}", request.Email);
                return StatusCode(500, new { message = "An error occurred during registration" });
            }
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<ActionResult<AuthResponse>> ChangePassword(ChangePasswordRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "User not authenticated" });

                var success = await _authService.ChangePasswordAsync(Guid.Parse(userId), request);
                if (!success)
                    return BadRequest(new { message = "Password change failed" });

                // Get updated user to generate new token
                var user = await _authService.GetUserByIdAsync(Guid.Parse(userId));
                var token = _jwtService.GenerateToken(user);

                return Ok(new AuthResponse { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during password change for user {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, new { message = "An error occurred during password change" });
            }
        }

    }
} 