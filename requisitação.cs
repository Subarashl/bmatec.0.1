// Seu código C# no método Index(string code) do seu Controller

// ... dentro do if (!string.IsNullOrEmpty(code))
// O valor de 'code' é: TG-690900459df8090001049e13-1761128516

// 1. Obter as configurações (Client ID, Secret, etc.)
// ... (obtido do appsettings.json ou User Secrets)

// 2. Preparar o conteúdo da requisição POST
var content = new FormUrlEncodedContent(new[]
{
    new KeyValuePair<string, string>("grant_type", "authorization_code"),
    new KeyValuePair<string, string>("client_id", _configuration["MercadoLivreApi:ClientId"]),
    new KeyValuePair<string, string>("client_secret", _configuration["MercadoLivreApi:ClientSecret"]),
    
    // **AQUI VOCÊ USA O CÓDIGO RECEBIDO**
    new KeyValuePair<string, string>("code", code), 
    
    new KeyValuePair<string, string>("redirect_uri", _configuration["MercadoLivreApi:RedirectUri"])
});

// 3. Fazer a requisição POST para o Mercado Livre
// ... (continua com a chamada ao tokenUrl)