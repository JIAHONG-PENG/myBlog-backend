using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("/me")]
public class UserController : ControllerBase
{
    [HttpGet]
    // [Authorize]
    public IActionResult GetCurrentUser()
    {
        var username = User.Identity?.IsAuthenticated == true ? User.Identity.Name : null;

        return Ok(new { username, login = username != null });
    }
}