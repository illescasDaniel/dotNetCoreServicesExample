using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AutoMapper;
using myMicroservice.Database;

namespace myMicroservice.Models
{
    public struct User
    {

        // Properties

        /// <example>2</example>
        public int Id { get; set; }

        /// <example>"Daniel"</example>
        [MaxLength(40)]
        public string Username { get; set; }

        /// <example>"example@email.com"</example>
        [MaxLength(50)]
        public string Email { get; set; }

        public string? Token { get; set; }

        // Methods

        public Database.Entities.User MapToUserEntity() // might not be used, should't be used when updating for example, we might use another entity instead
        {
            var mapperConfig = new MapperConfiguration(config =>
                config.CreateMap<User, Database.Entities.User>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            );
            var mapper = new Mapper(mapperConfig);
            return mapper.Map<Database.Entities.User>(this);
        }

        // Initializers

        public User(Database.Entities.User userEntity, string? token = null)
        {
            var mapperConfig = new MapperConfiguration(config =>
                config.CreateMap<Database.Entities.User, User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Token, opt => opt.MapFrom(src => token))
            );
            var mapper = new Mapper(mapperConfig);
            this = mapper.Map<User>(userEntity);
        }

        public User(int id, string email, string username)
        {
            Id = id;
            Username = username;
            Email = email;
            Token = null;
        }

        public User(int id, string email, string username, string token)
        {
            Id = id;
            Username = username;
            Email = email;
            Token = token;
        }
    }
}
