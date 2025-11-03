using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "API da Bmatec conectada!");

app.MapGet("/callback", (HttpRequest req) =>
{
    var code = req.Query["code"];
    return $"Código recebido do Mercado Livre: {code}";
});

app.Run();
