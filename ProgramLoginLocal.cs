
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting();
builder.Services.AddDirectoryBrowser();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapGet("/", async context =>
{
    context.Response.ContentType = "text/html";
    await context.Response.SendFileAsync("wwwroot/login.html");
});

app.MapPost("/login", async context =>
{
    var form = await context.Request.ReadFormAsync();
    var username = form["username"];
    var password = form["password"];

    var json = await File.ReadAllTextAsync("users.json");
    var users = JsonSerializer.Deserialize<List<Usuario>>(json);

    if (users != null && users.Any(u => u.username == username && u.password == password))
    {
        await context.Response.WriteAsync("<h2>✅ Login realizado com sucesso!</h2>");
    }
    else
    {
        await context.Response.WriteAsync("<h2>❌ Usuário ou senha inválidos.</h2>");
    }
});

app.Run("http://0.0.0.0:5000");

public class Usuario
{
    public string username { get; set; }
    public string password { get; set; }
}
