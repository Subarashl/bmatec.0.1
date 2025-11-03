using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Endpoint raiz
app.MapGet("/", () => "API da Bmatec conectada!");

// Classe para desserializar a resposta do Mercado Livre
public class TokenResponse
{
    public string AccessToken { get; set; }
    public string TokenType { get; set; }
    public long ExpiresIn { get; set; }
    public string Scope { get; set; }
    public long UserId { get; set; }
    public string RefreshToken { get; set; }
    // Propriedades adicionais para erros, se necessário
    public string Error { get; set; }
    public string ErrorDescription { get; set; }
}

// Endpoint callback do Mercado Livre
app.MapGet("/callback", async (HttpRequest req) =>
{
    var code = req.Query["code"];
    if (string.IsNullOrEmpty(code))
        return Results.BadRequest("Código de autorização não fornecido na URL.");

    // !!! ATENÇÃO: Use nomes de variáveis de ambiente consistentes !!!
    // Exemplo: MERCADOLIVRE_CLIENT_ID, MERCADOLIVRE_CLIENT_SECRET, MERCADOLIVRE_REDIRECT_URI
    var appId = Environment.GetEnvironmentVariable("MERCADOLIVRE_CLIENT_ID") ?? throw new Exception("MERCADOLIVRE_CLIENT_ID não configurado!");
    var secretKey = Environment.GetEnvironmentVariable("MERCADOLIVRE_CLIENT_SECRET") ?? throw new Exception("MERCADOLIVRE_CLIENT_SECRET não configurado!");
    
    // Opcional: ler redirectUri da variável de ambiente também
    var redirectUri = Environment.GetEnvironmentVariable("MERCADOLIVRE_REDIRECT_URI") ?? "https://bmatec.onrender.com/callback";

    // Troca code por access_token
    using var client = new HttpClient();
    var content = new FormUrlEncodedContent(new[]
    {
        new KeyValuePair<string, string>("grant_type", "authorization_code"),
        new KeyValuePair<string, string>("client_id", appId),
        new KeyValuePair<string, string>("client_secret", secretKey),
        new KeyValuePair<string, string>("code", code),
        new KeyValuePair<string, string>("redirect_uri", redirectUri)
    });

    var response = await client.PostAsync("https://api.mercadolibre.com/oauth/token", content);
    var jsonResponse = await response.Content.ReadAsStringAsync();

    // Verifica se a requisição foi bem sucedida
    if (response.IsSuccessStatusCode)
    {
        var tokenData = JsonSerializer.Deserialize<TokenResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        // !!! PASSO CRÍTICO DE SEGURANÇA !!!
        // Em vez de retornar para o navegador, armazene tokenData.AccessToken e tokenData.RefreshToken 
        // em um banco de dados ou outro armazenamento persistente seguro.
        Console.WriteLine($"Token obtido com sucesso para o usuário {tokenData.UserId}. Armazenando no BD...");

        // Redirecione o usuário para uma página de sucesso
        return Results.Redirect("/success"); 
    }
    else
    {
        // Tratar o erro de forma apropriada (logar, exibir mensagem de erro)
        Console.Error.WriteLine($"Erro na troca de token: {jsonResponse}");
        return Results.StatusCode((int)response.StatusCode);
    }
});

// Endpoint de sucesso após o callback
app.MapGet("/success", () => "Autenticação com Mercado Livre realizada com sucesso! Os tokens foram armazenados.");

// Endpoint para o Arduino pegar o token (simples exemplo)
// Este endpoint deve ler o token do seu armazenamento persistente, não de uma string fixa.
app.MapGet("/arduino-token", () =>
{
    // AQUI você leria o token do seu BD/Redis
    string storedToken = Environment.GetEnvironmentVariable("ULTIMO_ACCESS_TOKEN_SALVO") ?? "TOKEN_NAO_ENCONTRADO";
    return Results.Json(new { access_token = storedToken });
});

app.Run();
