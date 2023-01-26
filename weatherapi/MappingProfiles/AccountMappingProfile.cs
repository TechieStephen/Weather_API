using weatherapi.Entities;
using weatherapi.Models.Account;
using AutoMapper;

namespace weatherapi.MappingProfiles
{
    public class AccountMappingProfile : Profile
    {
        public AccountMappingProfile()
        {
            // Add as many of these lines as you need to map defferent objects relating to user account 
            CreateMap<ApplicationUser, UserDto>();
        }
    }

}
