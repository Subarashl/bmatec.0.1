using System.Text.Json.Serialization; // Necessário para os atributos

namespace Bmatec.Models // <-- Use um namespace de Modelos
{
    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        
        // ... (o restante das propriedades)

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }
        
        [JsonPropertyName("error_description")]
        public string ErrorDescription { get; set; }
    }
}