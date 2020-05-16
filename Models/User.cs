using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace myMicroservice.Models
{
    public struct User
    {
        /// <example>2</example>
        public int Id { get; set; }

        /// <example>"Daniel"</example>
        [MaxLength(40)]
        public string Username { get; set; }

        /// <example>"example@email.com"</example>
        [MaxLength(50)]
        public string Email { get; set; }

        public string? Token { get; set; }

        public User(int id, string email, string username)
        {
            Id = id;
            Username = username;
            Email = email;
            Token = null;
        }

        public User(int id, string email, string username, string token)
        {
            Id = id;
            Username = username;
            Email = email;
            Token = token;
        }
    }
}
