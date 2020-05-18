using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace myMicroservice.Api.V1.Models
{
    public struct UpdatedDeviceDto
    {
        /// <example>iOS</example>
        [MaxLength(40)]
        [Required]
        public string Name { get; set; }

        /// <example>13.4.1</example>
        [MaxLength(50)]
        [Required]
        public string Version { get; set; }

        public UpdatedDeviceDto(string name, string version)
        {
            Name = name;
            Version = version;
        }
    }
}
