using Microsoft.AspNetCore.Mvc;
using Npgsql;
// using MySql.Data.MySqlClient;

[ApiController]
[Route("/signup")]
public class SignupController : ControllerBase
{
    private readonly NpgsqlConnection _connection;

    public SignupController(NpgsqlConnection connection)
    {
        _connection = connection;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] SignUpFormData data)
    {
        await _connection.OpenAsync();

        using var cmd = new NpgsqlCommand("SELECT * FROM Users WHERE username = @username", _connection);
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

            using var insertCmd = new NpgsqlCommand("INSERT INTO Users (username, date, password) VALUES (@username, @date, @password)", _connection);
            insertCmd.Parameters.AddWithValue("@username", data.username);
            insertCmd.Parameters.AddWithValue("@date", DateTime.Parse(data.date));
            insertCmd.Parameters.AddWithValue("@password", data.password);

            var affectedRow = await insertCmd.ExecuteNonQueryAsync();

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