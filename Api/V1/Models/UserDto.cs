using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace myMicroservice.Api.v1.Models
{
    public struct UserDto
    {

        // Properties

        /// <example>2</example>
        [Required]
        public int Id { get; set; }

        /// <example>Daniel</example>
        [MaxLength(40)]
        [Required]
        public string Username { get; set; }

        /// <example>Daniel</example>
        [MaxLength(40)]
        [Required]
        public string Name { get; set; }

        /// <example>Daniel</example>
        [MaxLength(50)]
        [Required]
        public string Surname { get; set; }

        /// <example>example@email.com</example>
        [MaxLength(50)]
        [Required]
        public string Email { get; set; }

        // Initializers

        public UserDto(Database.Entities.User userEntity)
        {
            var mapperConfig = new MapperConfiguration(config =>
                config.CreateMap<Database.Entities.User, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId))
            );
            var mapper = new Mapper(mapperConfig);
            this = mapper.Map<UserDto>(userEntity);
        }

        public UserDto(string email, string username, string name, string surname)
        {
            Id = 0;
            Username = username;
            Email = email;
            Name = name;
            Surname = surname;
        }
    }
}
