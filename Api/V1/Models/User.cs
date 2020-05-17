using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace myMicroservice.Api.v1.Models
{
    public struct User
    {

        // Properties

        /// <example>2</example>
        [Required]
        public int Id { get; set; }

        /// <example>Daniel</example>
        [MaxLength(40)]
        [Required]
        public string Username { get; set; }

        /// <example>example@email.com</example>
        [MaxLength(50)]
        [Required]
        public string Email { get; set; }

        // Methods

        public Database.Entities.UserEntity MapToUserEntity() // might not be used, should't be used when updating for example, we might use another entity instead
        {
            var mapperConfig = new MapperConfiguration(config =>
                config.CreateMap<User, Database.Entities.UserEntity>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            );
            var mapper = new Mapper(mapperConfig);
            return mapper.Map<Database.Entities.UserEntity>(this);
        }

        // Initializers

        public User(Database.Entities.UserEntity userEntity)
        {
            var mapperConfig = new MapperConfiguration(config =>
                config.CreateMap<Database.Entities.UserEntity, User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId))
            );
            var mapper = new Mapper(mapperConfig);
            this = mapper.Map<User>(userEntity);
        }

        public User(int id, string email, string username)
        {
            Id = id;
            Username = username;
            Email = email;
        }

        public User(int id, string email, string username, string token)
        {
            Id = id;
            Username = username;
            Email = email;
        }
    }
}
