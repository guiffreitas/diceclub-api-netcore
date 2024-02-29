using AutoMapper;
using diceclub_api_netcore.Domain.Entities;
using diceclub_api_netcore.Dtos;

namespace diceclub_api_netcore.Profiles
{
    public class UserMap : Profile
    {
        public UserMap() 
        {
            CreateMap<UserDto, User>();
        }
    }
}
