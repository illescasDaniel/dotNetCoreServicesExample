using System;
namespace myMicroservice.Properties
{
    public class AppSettings
    {

        public string? JwtSecret { get; set; }

        public AppSettings()
        {
            JwtSecret = null;
        }

        public AppSettings(string jwtSecret)
        {
            JwtSecret = jwtSecret;
        }
    }
}
