using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

[ApiController]
[Route("/comment")]
public class CommentsController : ControllerBase
{
    private readonly MySqlConnection _connection;

    public CommentsController(MySqlConnection connection)
    {
        _connection = connection;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string logId)
    {
        await _connection.OpenAsync();

        var cmd = new MySqlCommand("SELECT * FROM Comment WHERE logId = @logId", _connection);
        cmd.Parameters.AddWithValue("@logId", logId);

        using var reader = await cmd.ExecuteReaderAsync();
        var comments = new List<Object>();

        while (await reader.ReadAsync())
        {
            comments.Add(new
            {
                username = reader.GetString(reader.GetOrdinal("username")),
                date = reader.GetDateTime(reader.GetOrdinal("date")).ToString("yyyy-MM-dd HH:mm"),
                content = reader.GetString(reader.GetOrdinal("content")),
            });
        }

        await _connection.CloseAsync();

        return Ok(comments);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CommentBodyData data)
    {
        await _connection.OpenAsync();

        var cmd = new MySqlCommand("INSERT INTO Comment (logId, username, date, content) VALUES (@logId, @username, @date, @content)", _connection);
        cmd.Parameters.AddWithValue("@logId", data.logId);
        cmd.Parameters.AddWithValue("@username", data.username);
        cmd.Parameters.AddWithValue("@date", data.date);
        cmd.Parameters.AddWithValue("@content", data.content);

        var affectedRow = await cmd.ExecuteNonQueryAsync();

        await _connection.CloseAsync();

        return Ok(new { affectedRow });
    }
}

public class CommentBodyData
{
    public int logId { get; set; }
    public string username { get; set; }
    public string date { get; set; }
    public string content { get; set; }

}