using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using myMicroservice.Properties;

namespace myMicroservice.Helpers
{

    public interface IUserAuthenticationService
    {
        string Authenticate(int userId);
    }

    public class UserAuthenticationService : IUserAuthenticationService
    {

        private readonly AppSettings _appSettings;

        public UserAuthenticationService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        /// <summary>
        /// Creates a token for a given user
        /// </summary>
        /// <param name="userId">User Id, to use it as a 'Claim' </param>
        /// <returns>Authentication token</returns>
        public string Authenticate(int userId)
        {
            var secret = _appSettings.JwtSecret;

            if (secret == null)
            {
                throw new Exception("JWT Secret is null!");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, userId.ToString()) // we might need to change / add / remove these
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
