using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class HomeController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public HomeController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    // Este método irá lidar com o retorno do Mercado Livre: https://bmatec.onrender.com/?code=...
    public async Task<IActionResult> Index(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            // Acesso normal à raiz do site.
            return View(); 
        }

        // 1. Obter as configurações (Client ID, Secret, etc.)
        var clientId = _configuration["MercadoLivreApi:ClientId"];
        var clientSecret = _configuration["MercadoLivreApi:ClientSecret"];
        var redirectUri = _configuration["MercadoLivreApi:RedirectUri"];
        var tokenUrl = _configuration["MercadoLivreApi:TokenUrl"];

        // 2. Preparar o conteúdo da requisição POST
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("redirect_uri", redirectUri)
        });

        // 3. Fazer a requisição POST para o Mercado Livre
        var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.PostAsync(tokenUrl, content);

        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            
            // 4. Deserializar e salvar os tokens
            var tokenData = JsonSerializer.Deserialize<TokenResponse>(jsonString);
            
            // --- AÇÃO CRUCIAL: SALVAR NO BANCO DE DADOS ---
            // Salve tokenData.access_token, tokenData.refresh_token, e tokenData.user_id
            
            // 5. Redirecionar
            return RedirectToAction("Dashboard");
        }
        else
        {
            // Lidar com erro
            var errorContent = await response.Content.ReadAsStringAsync();
            return Content($"Erro ao obter token do ML: {response.StatusCode}. Detalhe: {errorContent}");
        }
    }
    
    // ... Definição da classe TokenResponse em outro lugar para desserialização
    public class TokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string scope { get; set; }
        public long user_id { get; set; }
        public string refresh_token { get; set; }
    }
}   