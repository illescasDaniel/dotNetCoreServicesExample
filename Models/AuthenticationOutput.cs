namespace myMicroservice.Models
{
    public struct AuthenticationOutput
    {
        public string AccessToken { get; set; }

        //[JsonPropertyName("refresh_token")]
        //public string RefreshToken { get; set; } // TODO

        //[JsonPropertyName("id_token")]
        //public string TokenId { get; set; } // TODO

        public int UserId { get; set; }

        public string TokenType { get; set; }

        public AuthenticationOutput(
            string accessToken,
            //string refreshToken,
            //string tokenId,
            int userId,
            string tokenType = "Bearer"
        )
        {
            AccessToken = accessToken;
            //RefreshToken = refreshToken;
            //TokenId = tokenId;
            UserId = userId;
            TokenType = tokenType;
        }
    }
}
