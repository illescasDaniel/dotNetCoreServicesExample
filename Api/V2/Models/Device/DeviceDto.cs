using System.ComponentModel.DataAnnotations;
using ProtoBuf;

namespace myMicroservice.Api.V2.Models
{
    [ProtoContract]
    public struct DeviceDto
    {
        [Required]
        [ProtoMember(1)]
        public int Id { get; set; }

        /// <example>iOS</example>
        [MaxLength(40)]
        [Required]
        [ProtoMember(2)]
        public string Name { get; set; }

        /// <example>13.4.1</example>
        [MaxLength(50)]
        [Required]
        [ProtoMember(3)]
        public string Version { get; set; }

        [Required]
        [ProtoMember(4)]
        public int OwnerUserId { get; set; }

        public DeviceDto(string name, string version, int ownerUserId)
        {
            Id = 0;
            Name = name;
            Version = version;
            OwnerUserId = ownerUserId;
        }
    }
}
