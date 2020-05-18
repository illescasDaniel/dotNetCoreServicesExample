using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace myMicroservice.Api.V1.Models
{
    public struct DeviceDto
    {
        [Required]
        public int Id { get; set; }

        /// <example>iOS</example>
        [MaxLength(40)]
        [Required]
        public string Name { get; set; }

        /// <example>13.4.1</example>
        [MaxLength(50)]
        [Required]
        public string Version { get; set; }

        public int OwnerUserId { get; set; }

        //private static MapperConfiguration MapperConfig() => new MapperConfiguration(config =>
        //    config.CreateMap<Database.Entities.Device, DeviceDto>()
        //    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DeviceId))
        //);

        // Initializers

        //public DeviceDto(Database.Entities.Device deviceEntity)
        //{
        //    var mapper = new Mapper(MapperConfig());
        //    this = mapper.Map<DeviceDto>(deviceEntity);
        //}

        //public static List<DeviceDto> DevicesFromEntity(ICollection<Database.Entities.Device> devices)
        //{
        //    var mapper = new Mapper(MapperConfig());
        //    return mapper.Map<List<DeviceDto>>(devices);
        //}

        public DeviceDto(string name, string version, int ownerUserId)
        {
            Id = 0;
            Name = name;
            Version = version;
            OwnerUserId = ownerUserId;
        }
    }
}
