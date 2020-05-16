using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace myMicroservice.Models
{
    public struct RegistrationModel
    {
        /// <example>"Daniel"</example>
        [MaxLength(40)]
        [Required]
        public string Username { get; set; }

        /// <example>"ultra-secure-pass"</example>
        [Required]
        public string Password { get; set; }

        /// <example>"example@gmail.com"</example>
        [MaxLength(50)]
        [Required]
        public string Email { get; set; }

        public Database.Entities.User MapToUserEntity() // might not be used, should't be used when updating for example, we might use another entity instead
        {
            var mapperConfig = new MapperConfiguration(config =>
                config.CreateMap<RegistrationModel, Database.Entities.User>()
                .ForMember(dest => dest.HashedPassword, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => 0))
            );
            var mapper = new Mapper(mapperConfig);
            return mapper.Map<Database.Entities.User>(this);
        }
    }
}
