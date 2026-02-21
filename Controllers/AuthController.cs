using DevOpsReleasePortal.Auth;
using DevOpsReleasePortal.Data;
using DevOpsReleasePortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DevOpsReleasePortal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthController(UserManager<ApplicationUser> userManager, IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized(new { message = "Invalid credentials." });
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtTokenService.CreateToken(user, roles, out var expiresAtUtc);

        return Ok(new AuthResponse
        {
            Token = token,
            Email = user.Email ?? string.Empty,
            Roles = roles,
            ExpiresAtUtc = expiresAtUtc
        });
    }

    [Authorize(Roles = "Developer,DevOps,Tester,Manager,BA")]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            Email = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email || c.Type == "email")?.Value,
            Roles = User.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToArray()
        });
    }
}
