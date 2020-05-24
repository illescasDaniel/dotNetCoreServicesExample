using System.ComponentModel.DataAnnotations;
using ProtoBuf;

namespace myMicroservice.Api.V2.Models
{
    [ProtoContract]
    public class UpdatedUserDto
    {
        #region Properties

        /// <example>Daniel</example>
        [MaxLength(40)]
        [ProtoMember(1)]
        public string Name { get; set; } = null!;

        /// <example>Daniel</example>
        [MaxLength(50)]
        [ProtoMember(2)]
        public string Surname { get; set; } = null!;

        /// <example>example@email.com</example>
        [MaxLength(50)]
        [ProtoMember(3)]
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
