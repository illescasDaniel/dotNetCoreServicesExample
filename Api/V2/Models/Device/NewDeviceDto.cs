using System.ComponentModel.DataAnnotations;
using AutoMapper;
using ProtoBuf;

namespace myMicroservice.Api.V2.Models
{
    [ProtoContract]
    public struct NewDeviceDto
    {
        /// <example>iOS</example>
        [MaxLength(40)]
        [Required]
        [ProtoMember(1)]
        public string Name { get; set; }

        /// <example>13.4.1</example>
        [MaxLength(50)]
        [Required]
        [ProtoMember(2)]
        public string Version { get; set; }

        // Methods

        public Database.Entities.Device MapToDeviceEntity() // might not be used, should't be used when updating for example, we might use another entity instead
        {
            var mapperConfig = new MapperConfiguration(config =>
                config.CreateMap<NewDeviceDto, Database.Entities.Device>()
                .ForMember(dest => dest.DeviceId, opt => opt.Ignore())
                .ForMember(dest => dest.Owner, opt => opt.Ignore())
                .ForMember(dest => dest.OwnerUserId, opt => opt.Ignore())
            );
            var mapper = new Mapper(mapperConfig);
            return mapper.Map<Database.Entities.Device>(this);
        }

        public NewDeviceDto(string name, string version)
        {
            Name = name;
            Version = version;
        }
    }
}
