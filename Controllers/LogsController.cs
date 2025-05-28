using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

[Route("/logs")]
[ApiController]
public class LogsController : ControllerBase
{
    private readonly MySqlConnection _connection;

    public LogsController(MySqlConnection connection)
    {
        _connection = connection;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {

        await _connection.OpenAsync();

        var cmd = new MySqlCommand("SELECT * FROM Log", _connection);
        using var reader = await cmd.ExecuteReaderAsync();

        var logs = new List<Object>();
        while (await reader.ReadAsync())
        {
            logs.Add(new
            {
                logId = reader.GetInt16(reader.GetOrdinal("logId")),
                username = reader.GetString(reader.GetOrdinal("username")),
                title = reader.GetString(reader.GetOrdinal("title")),
                date = reader.GetDateTime(reader.GetOrdinal("date")).ToString("yyyy-MM-dd HH:mm"),
                content = reader.GetString(reader.GetOrdinal("content")),
            });
        }

        await _connection.CloseAsync();

        return Ok(logs);
    }

    public async Task<IActionResult> Post([FromBody] FormData data)
    {

        await _connection.OpenAsync();

        var cmd = new MySqlCommand("INSERT INTO Log (username, title, date, content) VALUES (@username, @title, @date, @content)", _connection);

        cmd.Parameters.AddWithValue("@username", data.username);
        cmd.Parameters.AddWithValue("@title", data.title);
        cmd.Parameters.AddWithValue("@date", data.date);
        cmd.Parameters.AddWithValue("@content", data.content);

        int affectedRow = await cmd.ExecuteNonQueryAsync();

        await _connection.CloseAsync();

        return Ok(new { affectedRow });

    }

}

public class FormData
{
    public string username { get; set; }
    public string title { get; set; }
    public string date { get; set; }
    public string content { get; set; }
}