using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace myMicroservice.Database.Entities
{
    public class User
    {
        /// <example>1</example>
        [Key]
        [Required]
        public int UserId { get; set; }

        // is unique
        /// <example>Daniel69</example>
        [MaxLength(40)]
        [Required]
        public string Username { get; set; } = null!;

        /// <example>Daniel</example>
        [MaxLength(40)]
        [Required]
        public string Name { get; set; } = null!;

        /// <example>Daniel</example>
        [MaxLength(50)]
        [Required]
        public string Surname { get; set; } = null!;

        /// <example>xxxxxxxx</example>
        [Required]
        public string HashedPassword { get; set; } = null!;

        /// <example>example@gmail.com</example>
        [MaxLength(50)]
        [Required]
        public string Email { get; set; } = null!;

        // Navigation properties
        [InverseProperty("Owner")]
        public virtual ICollection<Device> Devices { get; set; } = null!;

        // Initializers

        public User()
        {}

        public User(string username, string hashedPassword, string email, string name, string surname)
        {
            Username = username;
            HashedPassword = hashedPassword;
            Email = email;
            Name = name;
            Surname = surname;
            Devices = new List<Device>();
        }

        public User(string username, string hashedPassword, string email, string name, string surname, List<Device> devices)
        {
            Username = username;
            HashedPassword = hashedPassword;
            Email = email;
            Name = name;
            Surname = surname;
            Devices = devices;
        }
    }
}
