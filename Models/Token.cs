using System;
using System.Text.Json.Serialization;

namespace myMicroservice.Models
{
    public struct Token
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        //[JsonPropertyName("refresh_token")]
        //public string RefreshToken { get; set; } // TODO

        //[JsonPropertyName("id_token")]
        //public string TokenId { get; set; } // TODO

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        public Token(
            string accessToken,
            //string refreshToken,
            //string tokenId,
            string tokenType = "Bearer"
        )
        {
            AccessToken = accessToken;
            //RefreshToken = refreshToken;
            //TokenId = tokenId;
            TokenType = tokenType;
        }
    }
}
