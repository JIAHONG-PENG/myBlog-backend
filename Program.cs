using Microsoft.AspNetCore.Authentication.Cookies;
using MySql.Data.MySqlClient;


var builder = WebApplication.CreateBuilder(args);

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy
            // .AllowAnyOrigin()     // allow all origins
            .WithOrigins("https://myblog66.netlify.app", "http://localhost:3000")
            .AllowAnyMethod()     // allow GET, POST, PUT, etc.
            .AllowAnyHeader()    // allow all headers
            .AllowCredentials();
    });
});


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401; // prevent redirect
                    return Task.CompletedTask;
                };
                // options.LoginPath = "/login"; 
                // options.LogoutPath = "/Account/Logout";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
            });

builder.Services.AddControllers();

builder.Services.AddTransient<MySqlConnection>(_ =>
    new MySqlConnection(builder.Configuration.GetConnectionString("Default")));


var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthentication(); // Adds authentication middleware
app.UseAuthorization();  // Adds authorization middleware
app.MapControllers();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.MapGet("/", () =>
{
    return "Silence is gold.";
});

app.Run();