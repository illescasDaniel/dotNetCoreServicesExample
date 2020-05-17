using System.ComponentModel.DataAnnotations;

namespace myMicroservice.Api.v1.Models
{
    public struct AuthenticationModel
    {
        /// <example>Daniel</example>
        [MaxLength(40)]
        [Required]
        public string Username { get; set; }

        /// <example>ultra-secure-pass</example>
        // here we might use a regex for the pass
        //[RegularExpression("......{1,10}")]
        [Required]
        public string Password { get; set; }
    }
}
