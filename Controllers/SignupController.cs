using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

[Route("/signup")]
[ApiController]
public class SignupController : ControllerBase
{
    private readonly MySqlConnection _connection;

    public SignupController(MySqlConnection connection)
    {
        _connection = connection;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] SignUpFormData data)
    {
        await _connection.OpenAsync();

        var cmd = new MySqlCommand("SELECT * FROM User WHERE username = @username", _connection);
        cmd.Parameters.AddWithValue("@username", data.username);
        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            await _connection.CloseAsync();

            return Ok(new { affectedRow = 0 });
        }
        else
        {
            await reader.CloseAsync();

            cmd = new MySqlCommand("INSERT INTO User (username, date, password) VALUES (@username, @date, @password)", _connection);
            cmd.Parameters.AddWithValue("@username", data.username);
            cmd.Parameters.AddWithValue("@date", data.date);
            cmd.Parameters.AddWithValue("@password", data.password);

            var affectedRow = await cmd.ExecuteNonQueryAsync();

            await _connection.CloseAsync();

            return Ok(new { affectedRow });
        }

    }

}

public class SignUpFormData
{
    public string username { get; set; }
    public string date { get; set; }
    public string password { get; set; }
}