using System.ComponentModel.DataAnnotations;

namespace myMicroservice.Models
{
    public struct RegistrationModel
    {
        /// <example>"Daniel"</example>
        [MaxLength(40)]
        [Required]
        public string Username { get; set; }

        /// <example>"ultra-secure-pass"</example>
        [Required]
        public string Password { get; set; }

        /// <example>"example@gmail.com"</example>
        [MaxLength(50)]
        [Required]
        public string Email { get; set; }
    }
}
