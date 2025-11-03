using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Endpoint raiz
app.MapGet("/", () => "API da Bmatec conectada!");

// Endpoint callback do Mercado Livre
app.MapGet("/callback", async (HttpRequest req) =>
{
    var code = req.Query["code"];
    if (string.IsNullOrEmpty(code))
        return Results.BadRequest("Código não fornecido");

    // Lê credenciais do Render
   var appId = Environment.GetEnvironmentVariable("APP_ID") ?? throw new Exception("APP_ID não configurado!");
    var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY") ?? throw new Exception("SECRET_KEY não configurado!");
    var redirectUri = "https://bmatec.onrender.com/callback";

    // Troca code por access_token
    using var client = new HttpClient();
    var content = new FormUrlEncodedContent(new[]
    {
        new KeyValuePair<string,string>("grant_type","authorization_code"),
        new KeyValuePair<string,string>("client_id", appId),
        new KeyValuePair<string,string>("client_secret", secretKey),
        new KeyValuePair<string,string>("code", code),
        new KeyValuePair<string,string>("redirect_uri", redirectUri)
    });

    var response = await client.PostAsync("https://api.mercadolibre.com/oauth/token", content);
    var tokenJson = await response.Content.ReadAsStringAsync();

    // Retorna token como JSON (ou armazene em memória / banco)
    return Results.Content(tokenJson, "application/json");
});

// Endpoint para o Arduino pegar o token (simples exemplo)
app.MapGet("/arduino-token", () =>
{
    // Aqui você pode ler de memória ou banco de dados
    return Results.Json(new { access_token = "EXEMPLO_DE_TOKEN" });
});

app.Run();
