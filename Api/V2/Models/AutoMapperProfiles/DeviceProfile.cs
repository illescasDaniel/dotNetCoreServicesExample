using AutoMapper;
using myMicroservice.Database.Entities;

namespace myMicroservice.Api.V2.Models.AutoMapperProfiles
{
    public class DeviceProfile : Profile
    {
        public DeviceProfile()
        {
            CreateMap<Device, DeviceDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DeviceId));

            CreateMap<Device, UpdatedDeviceDto>();

            CreateMap<NewDeviceDto, Device>()
                .ForMember(dest => dest.DeviceId, opt => opt.Ignore())
                .ForMember(dest => dest.Owner, opt => opt.Ignore())
                .ForMember(dest => dest.OwnerUserId, opt => opt.Ignore());
        }
    }
}
