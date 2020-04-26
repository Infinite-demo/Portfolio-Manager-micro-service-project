using AutoMapper;
using UserManagement.Domain;
using UserManagement.Model;

namespace UserManagement.Config
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
