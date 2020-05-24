using AutoMapper;
using myMicroservice.Database.Entities;

namespace myMicroservice.Api.V2.Models.AutoMapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId));

            CreateMap<User, UpdatedUserDto>();

            CreateMap<RegistrationModel, User>()
                .ForMember(dest => dest.HashedPassword, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.UserId, opt => opt.Ignore());
        }
    }
}
