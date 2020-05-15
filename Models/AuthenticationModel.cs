using System;
using System.ComponentModel.DataAnnotations;

namespace myMicroservice.Models
{
    public struct AuthenticationModel
    {
        // ... [MinLength]
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        //

        /// <summary>Constructor</summary>
        public AuthenticationModel(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
