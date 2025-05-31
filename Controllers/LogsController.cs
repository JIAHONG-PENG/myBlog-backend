using Microsoft.AspNetCore.Mvc;
// using MySql.Data.MySqlClient;
using Npgsql;

[ApiController]
[Route("/logs")]
public class LogsController : ControllerBase
{
    // private readonly MySqlConnection _connection;
    private readonly NpgsqlConnection _connection;
    // private readonly INpgsqlConnectionFactory _connectionFactory;

    public LogsController(NpgsqlConnection connection)
    {
        _connection = connection;
        // _connectionFactory = connectionFactory;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        await _connection.OpenAsync();

        using var cmd = new NpgsqlCommand("SELECT * FROM Log", _connection);
        using var reader = await cmd.ExecuteReaderAsync();

        var logs = new List<Object>();
        while (await reader.ReadAsync())
        {
            var date = reader.GetDateTime(reader.GetOrdinal("date"));
            var date_s = "";
            if (date.Year == DateTime.Now.Year)
            {
                date_s = date.ToString("MM-dd HH:mm");
            }

            logs.Add(new
            {
                logId = reader.GetInt16(reader.GetOrdinal("logId")),
                username = reader.GetString(reader.GetOrdinal("username")),
                title = reader.GetString(reader.GetOrdinal("title")),
                date = date_s,
                content = reader.GetString(reader.GetOrdinal("content")),
            });
        }

        await _connection.CloseAsync();

        return Ok(logs);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] FormPostData data)
    {
        try
        {

            await _connection.OpenAsync();

            using var cmd = new NpgsqlCommand("INSERT INTO Log (username, title, date, content) VALUES (@username, @title, @date, @content)", _connection);

            cmd.Parameters.AddWithValue("@username", data.username);
            cmd.Parameters.AddWithValue("@title", data.title);
            cmd.Parameters.AddWithValue("@date", DateTime.Parse(data.date));
            cmd.Parameters.AddWithValue("@content", data.content);

            int affectedRow = await cmd.ExecuteNonQueryAsync();

            await _connection.CloseAsync();

            return Ok(new { affectedRow });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] FormDeleteData data)
    {
        try
        {
            await _connection.OpenAsync();

            // delete command first
            using var cmd1 = new NpgsqlCommand("DELETE FROM Comment WHERE logId = @logId", _connection);
            cmd1.Parameters.AddWithValue("@logId", data.logId);

            await cmd1.ExecuteNonQueryAsync();

            // delete log
            using var cmd = new NpgsqlCommand("DELETE FROM Log WHERE logId = @logId", _connection);
            cmd.Parameters.AddWithValue("@logId", data.logId);

            await cmd.ExecuteNonQueryAsync();

            await _connection.CloseAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }

    }

}

public class FormPostData
{
    public string username { get; set; }
    public string title { get; set; }
    public string date { get; set; }
    public string content { get; set; }
}

public class FormDeleteData
{
    public int logId { get; set; }
}