using System.ComponentModel.DataAnnotations;

namespace myMicroservice.Database.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [MaxLength(40)]
        public string Username { get; set; } = null!; // should be unique, maybe it should be part of the primary key

        public string HashedPassword { get; set; } = null!;

        [MaxLength(50)]
        public string Email { get; set; } = null!;

        // data annotations are valid here too, like "Required", "maxlength, etc"

        public User(string username, string hashedPassword, string email)
        {
            Username = username;
            HashedPassword = hashedPassword;
            Email = email;
        }
    }
}
