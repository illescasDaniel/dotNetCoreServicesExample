using System.ComponentModel.DataAnnotations;

namespace myMicroservice.Api.V1.Models
{
    public struct UserDto
    {

        #region Properties

        /// <example>2</example>
        [Required]
        public int Id { get; set; }

        /// <example>Daniel</example>
        [MaxLength(40)]
        [Required]
        public string Username { get; set; }

        /// <example>Daniel</example>
        [MaxLength(40)]
        [Required]
        public string Name { get; set; }

        /// <example>Daniel</example>
        [MaxLength(50)]
        [Required]
        public string Surname { get; set; }

        /// <example>example@email.com</example>
        [MaxLength(50)]
        [Required]
        public string Email { get; set; }

        #endregion

        #region Initializers

        public UserDto(string email, string username, string name, string surname)
        {
            Id = 0;
            Username = username;
            Email = email;
            Name = name;
            Surname = surname;
        }

        #endregion
    }
}
