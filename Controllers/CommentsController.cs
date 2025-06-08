using Microsoft.AspNetCore.Mvc;
using Npgsql;
// using MySql.Data.MySqlClient;

[ApiController]
[Route("/comment")]
public class CommentsController : ControllerBase
{
    // private readonly MySqlConnection _connection;
    private readonly NpgsqlConnection _connection;

    public CommentsController(NpgsqlConnection connection)
    {
        _connection = connection;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string logId)
    {
        await _connection.OpenAsync();

        using var cmd = new NpgsqlCommand("SELECT * FROM Comment WHERE logId = @logId", _connection);
        cmd.Parameters.AddWithValue("@logId", int.Parse(logId));

        using var reader = await cmd.ExecuteReaderAsync();
        var comments = new List<Object>();

        while (await reader.ReadAsync())
        {
            var date = reader.GetDateTime(reader.GetOrdinal("date"));
            var date_s = "";
            if (date.Year == DateTime.Now.Year)
            {
                date_s = date.ToString("MM-dd HH:mm");
            }

            comments.Add(new
            {
                commentId = reader.GetInt16(reader.GetOrdinal("commentid")),
                parent_commentId = reader.IsDBNull(reader.GetOrdinal("parent_commentid")) ? (short?)null : reader.GetInt16(reader.GetOrdinal("parent_commentid")),
                logId = reader.GetInt16(reader.GetOrdinal("logid")),
                username = reader.GetString(reader.GetOrdinal("username")),
                date = date_s,
                content = reader.GetString(reader.GetOrdinal("content")),
            });
        }

        await _connection.CloseAsync();

        return Ok(comments);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CommentPostBodyData data)
    {
        await _connection.OpenAsync();

        var cmd = new NpgsqlCommand("INSERT INTO Comment (logId, parent_commentid, username, date, content) VALUES (@logId, @parent_commentid, @username, @date, @content)", _connection);
        cmd.Parameters.AddWithValue("@logId", data.logId);
        cmd.Parameters.AddWithValue("@parent_commentid", data.parent_commentId ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@username", data.username);
        cmd.Parameters.AddWithValue("@date", DateTime.Parse(data.date));
        cmd.Parameters.AddWithValue("@content", data.content);

        var affectedRow = await cmd.ExecuteNonQueryAsync();

        await _connection.CloseAsync();

        return Ok(new { affectedRow });
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] CommentDeleteBodyData data)
    {
        await _connection.OpenAsync();

        using var cmd = new NpgsqlCommand("DELETE FROM Comment WHERE logId = @logId AND commentId = @commentId", _connection);
        cmd.Parameters.AddWithValue("@logId", data.logId);
        cmd.Parameters.AddWithValue("@commentId", data.commentId);

        int affectedRow = await cmd.ExecuteNonQueryAsync();

        await _connection.CloseAsync();

        return Ok(new { affectedRow });

    }
}

public class CommentPostBodyData
{
    public int logId { get; set; }
    public int? parent_commentId { get; set; }
    public string username { get; set; }
    public string date { get; set; }
    public string content { get; set; }

}

public class CommentDeleteBodyData
{
    public int logId { get; set; }
    public int commentId { get; set; }
}