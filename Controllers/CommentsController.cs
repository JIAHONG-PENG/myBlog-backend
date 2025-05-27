using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

[Route("/comment")]
[ApiController]
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
}