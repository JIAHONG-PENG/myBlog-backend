using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
// using MySql.Data.MySqlClient;

[ApiController]
[Route("/login")]
public class LoginController : ControllerBase
{
    private readonly NpgsqlConnection _connection;

    public LoginController(NpgsqlConnection connection)
    {
        _connection = connection;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] LoginFormData data)
    {
        await _connection.OpenAsync();

        using var cmd = new NpgsqlCommand("SELECT * FROM Users WHERE username = @username AND password = @password", _connection);
        cmd.Parameters.AddWithValue("@username", data.username);
        cmd.Parameters.AddWithValue("@password", data.password);

        using var reader = await cmd.ExecuteReaderAsync();

        // find one matched user
        if (await reader.ReadAsync())
        {
            await _connection.CloseAsync();

            // add cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, data.username)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            return Ok(new { found = true, data.username, });
        }
        else
        {
            await reader.CloseAsync();

            await _connection.CloseAsync();

            return Ok(new { found = false });
        }
    }
}

public class LoginFormData
{
    public string username { get; set; }
    public string password { get; set; }

}