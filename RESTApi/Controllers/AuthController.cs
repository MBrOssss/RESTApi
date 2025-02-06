using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using RESTApi.Constants;
using RESTApi.Models;
using RESTApi.Models.DTOs;
using RESTApi.Services;
using System.Security.Claims;

namespace RESTApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthController> _logger;
        private readonly ITokenService _tokenService;
        private readonly ApplicationDbContext _context;


        public AuthController(
            UserManager<IdentityUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            ILogger<AuthController> logger,
            ITokenService tokenService,
            ApplicationDbContext context
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    return BadRequest("User already exists");
                }

                if (!await _roleManager.RoleExistsAsync(Roles.User))
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole(Roles.User));

                    if (!roleResult.Succeeded)
                    {
                        var roleErros = roleResult.Errors.Select(e => e.Description);
                        _logger.LogError($"Failed to create user role. Errors : {string.Join(",", roleErros)}");
                        return BadRequest($"Failed to create user role. Errors : {string.Join(",", roleErros)}");
                    }
                }

                IdentityUser user = new()
                {
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.Email,
                    EmailConfirmed = true
                };

                var createUserResult = await _userManager.CreateAsync(user, model.Password);

                if (!createUserResult.Succeeded)
                {
                    var errors = createUserResult.Errors.Select(e => e.Description);
                    _logger.LogError($"Failed to create user. Errors: {string.Join(", ", errors)}");
                    return BadRequest($"Failed to create user. Errors: {string.Join(", ", errors)}");
                }

                var addUserToRoleResult = await _userManager.AddToRoleAsync(user, Roles.User);

                if (addUserToRoleResult.Succeeded == false)
                {
                    var errors = addUserToRoleResult.Errors.Select(e => e.Description);
                    _logger.LogError($"Failed to add role to the user. Errors : {string.Join(",", errors)}");
                }
                return CreatedAtAction(nameof(Register), null);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return BadRequest("User with this username is not registered.");
                }
                bool isValidPassword = await _userManager.CheckPasswordAsync(user, model.Password);
                if (!isValidPassword)
                {
                    return Unauthorized();
                }

                List<Claim> authClaims = [
                    new (ClaimTypes.Name, user.UserName),
                    new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                ];

                var userRoles = await _userManager.GetRolesAsync(user);

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                string token = _tokenService.GenerateAccessToken(authClaims);
                string refreshToken = _tokenService.GenerateRefreshToken();

                var tokenInfo = _context.TokenInfos.FirstOrDefault(a => a.Username == user.UserName);
                if (tokenInfo == null)
                {
                    var ti = new TokenInfo
                    {
                        Username = user.UserName,
                        RefreshToken = refreshToken,
                        ExpiredAt = DateTime.UtcNow.AddDays(7)
                    };
                    _context.TokenInfos.Add(ti);
                }
                else
                {
                    tokenInfo.RefreshToken = refreshToken;
                    tokenInfo.ExpiredAt = DateTime.UtcNow.AddDays(7);
                }

                await _context.SaveChangesAsync();

                return Ok(new TokenModel
                {
                    AccessToken = token,
                    RefreshToken = refreshToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Unauthorized();
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(TokenModel tokenModel)
        {
            try
            {
                var principal = _tokenService.GetPrincipalFromExpiredToken(tokenModel.AccessToken);
                var username = principal.Identity.Name;

                var tokenInfo = _context.TokenInfos.SingleOrDefault(u => u.Username == username);
                if (tokenInfo == null || tokenInfo.RefreshToken != tokenModel.RefreshToken || tokenInfo.ExpiredAt <= DateTime.UtcNow)
                {
                    return BadRequest("Invalid refresh token. Please login again.");
                }

                var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                tokenInfo.RefreshToken = newRefreshToken;
                await _context.SaveChangesAsync();

                return Ok(new TokenModel
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("revoke")]
        [Authorize]
        public async Task<IActionResult> Revoke()
        {
            try
            {
                var username = User.Identity.Name;

                var user = _context.TokenInfos.SingleOrDefault(u => u.Username == username);
                if (user == null)
                {
                    return BadRequest();
                }

                user.RefreshToken = string.Empty;
                await _context.SaveChangesAsync();

                return Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
