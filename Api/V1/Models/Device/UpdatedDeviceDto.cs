using System.ComponentModel.DataAnnotations;

namespace myMicroservice.Api.V1.Models
{
    public class UpdatedDeviceDto
    {
        /// <example>iOS</example>
        [MaxLength(40)]
        public string Name { get; set; } = null!;

        /// <example>13.4.1</example>
        [MaxLength(50)]
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
