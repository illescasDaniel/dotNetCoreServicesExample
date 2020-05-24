using System.ComponentModel.DataAnnotations;
using ProtoBuf;

namespace myMicroservice.Api.V2.Models
{
    [ProtoContract]
    public class UpdatedDeviceDto
    {
        /// <example>iOS</example>
        [MaxLength(40)]
        [ProtoMember(1)]
        public string Name { get; set; } = null!;

        /// <example>13.4.1</example>
        [MaxLength(50)]
        [ProtoMember(2)]
        public string Version { get; set; } = null!;

        public UpdatedDeviceDto()
        {
        }

        public UpdatedDeviceDto(string name, string version)
        {
            Name = name;
            Version = version;
        }
    }
}
