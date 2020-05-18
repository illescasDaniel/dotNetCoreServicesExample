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

        public DeviceDto(string name, string version, int ownerUserId)
        {
            Id = 0;
            Name = name;
            Version = version;
            OwnerUserId = ownerUserId;
        }
    }
}
