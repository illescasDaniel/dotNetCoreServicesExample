using System.ComponentModel.DataAnnotations;

namespace myMicroservice.Database.Entities
{
    public class Device
    {
        /// <example>1</example>
        [Key]
        [Required]
        public int DeviceId { get; set; }

        /// <example>iOS</example>
        [MaxLength(40)]
        [Required]
        public string Name { get; set; } = null!;

        /// <example>13.4.1</example>
        [MaxLength(50)]
        [Required]
        public string Version { get; set; } = null!;

        public int OwnerUserId { get; set; }

        [Required] // in this case, we always want to have an owner, so we delete devices on cascade when deleting its owner
        public virtual User Owner { get; set; } = null!;

        public Device()
        {
        }

        public Device(int deviceId, string name, string version)
        {
            DeviceId = deviceId;
            Name = name;
            Version = version;
        }

        public Device(int deviceId, string name, string version, User owner)
        {
            DeviceId = deviceId;
            Name = name;
            Version = version;
            OwnerUserId = owner.UserId;
            Owner = owner;
        }
    }
}
