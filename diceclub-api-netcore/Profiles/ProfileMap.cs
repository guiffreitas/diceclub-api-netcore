﻿using AutoMapper;
using diceclub_api_netcore.Domain.Entities;
using diceclub_api_netcore.Dtos;

namespace diceclub_api_netcore.Profiles
{
    public class ProfileMap : Profile
    {
        public ProfileMap() 
        {
            CreateMap<UserProfileDto, UserProfile>();
            CreateMap<UserProfile, UserProfileDto>();
        }
    }
}
