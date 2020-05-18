﻿using AutoMapper;
using myMicroservice.Database.Entities;

namespace myMicroservice.Api.V1.Models.AutoMapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Device, DeviceDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DeviceId));
        }
    }
}