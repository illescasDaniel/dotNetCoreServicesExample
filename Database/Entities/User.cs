using System.ComponentModel.DataAnnotations;

namespace myMicroservice.Database.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        // is unique
        [MaxLength(40)]
        public string Username { get; set; } = null!;

        public string HashedPassword { get; set; } = null!;

        [MaxLength(50)]
        public string Email { get; set; } = null!;

        public User()
        {}

        public User(string username, string hashedPassword, string email)
        {
            Username = username;
            HashedPassword = hashedPassword;
            Email = email;
        }
    }
}
