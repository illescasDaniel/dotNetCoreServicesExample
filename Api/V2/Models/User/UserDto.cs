using System.ComponentModel.DataAnnotations;
using ProtoBuf;

namespace myMicroservice.Api.V2.Models
{
    [ProtoContract]
    public struct UserDto
    {

        #region Properties

        /// <example>2</example>
        [Required]
        [ProtoMember(1)]
        public int Id { get; set; }

        /// <example>Daniel</example>
        [MaxLength(40)]
        [Required]
        [ProtoMember(2)]
        public string Username { get; set; }

        /// <example>Daniel</example>
        [MaxLength(40)]
        [Required]
        [ProtoMember(3)]
        public string Name { get; set; }

        /// <example>Daniel</example>
        [MaxLength(50)]
        [Required]
        [ProtoMember(4)]
        public string Surname { get; set; }

        /// <example>example@email.com</example>
        [MaxLength(50)]
        [Required]
        [ProtoMember(5)]
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
