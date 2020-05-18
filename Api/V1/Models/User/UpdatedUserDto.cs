using System.ComponentModel.DataAnnotations;
using Microsoft.CodeAnalysis;

namespace myMicroservice.Api.V1.Models
{
    public class UpdatedUserDto
    {
        #region Properties

        /// <example>Daniel</example>
        [MaxLength(40)]
        public string Name { get; set; } = null!;

        /// <example>Daniel</example>
        [MaxLength(50)]
        public string Surname { get; set; } = null!;

        /// <example>example@email.com</example>
        [MaxLength(50)]
        public string Email { get; set; } = null!;
        #endregion

        #region Initializers

        public UpdatedUserDto()
        {
        }

        public UpdatedUserDto(string name, string surname, string email)
        {
            Email = email;
            Name = name;
            Surname = surname;
        }

        #endregion
    }
}
