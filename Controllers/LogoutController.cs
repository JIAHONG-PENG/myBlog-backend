using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
// using MySql.Data.MySqlClient;

[ApiController]
[Route("/logout")]
public class LogoutController : ControllerBase
{
    // private readonly MySqlConnection _connection;

    // public LogoutController(MySqlConnection connection)
    // {
    //     _connection = connection;
    // }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok();
    }
}