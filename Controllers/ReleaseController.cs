using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevOpsReleasePortal.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Developer,DevOps,Tester,Manager,BA")]
public class ReleaseController : ControllerBase
{
    [HttpGet("roles-check")]
    public IActionResult RolesCheck()
    {
        return Ok(new { message = "Authorized", roles = User.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Role).Select(c => c.Value) });
    }
}
