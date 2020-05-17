using System.ComponentModel.DataAnnotations;

namespace myMicroservice.Database.Entities
{
    public class UserEntity
    {
        /// <example>1</example>
        [Key]
        [Required]
        public int UserId { get; set; }

        // is unique
        /// <example>Daniel</example>
        [MaxLength(40)]
        [Required]
        public string Username { get; set; } = null!;

        /// <example>xxxxxxxx</example>
        [Required]
        public string HashedPassword { get; set; } = null!;

        /// <example>example@gmail.com</example>
        [MaxLength(50)]
        [Required]
        public string Email { get; set; } = null!;

        public UserEntity()
        {}

        public UserEntity(string username, string hashedPassword, string email)
        {
            Username = username;
            HashedPassword = hashedPassword;
            Email = email;
        }
    }
}
