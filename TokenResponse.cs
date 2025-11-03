// TokenResponse.cs

namespace Bmatec
{
    public class TokenResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("expires_in")]
        public long ExpiresIn { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("scope")]
        public string Scope { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("user_id")]
        public long UserId { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        // Propriedades adicionais para erros, se necessário
        [System.Text.Json.Serialization.JsonPropertyName("error")]
        public string Error { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("error_description")]
        public string ErrorDescription { get; set; }
    }
}
 