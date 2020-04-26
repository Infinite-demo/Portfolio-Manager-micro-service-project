using AutoMapper;
using System.Threading.Tasks;
using UserManagement.Domain;
using UserManagement.Model;
using UserManagement.Persistence;

namespace UserManagement.Service
{
    public interface IUserManagementService
    {
        Task<User> RegisterUser(UserDto userDto);
        Task<Result<User>> Get(string id);
        Task<FilteredResult> Get(int skip = 0, int pageLimit = 0);
    }

    public class UserManagementService : IUserManagementService
    {
        private readonly IMapper _mapper;
        public readonly IUserRepository userRepository;

        public UserManagementService(IMapper mapper, IUserRepository userRepository)
        {
            _mapper = mapper;
            this.userRepository = userRepository;
        }

        public async Task<User> RegisterUser(UserDto userDto)
        {
            return await userRepository.Add(_mapper.Map<User>(userDto));
        }

        public Task<Result<User>> Get(string id)
        {
            return userRepository.Get(id);
        }

        public async Task<FilteredResult> Get(int skip = 0, int pageLimit = 0)
        {
            return await userRepository.Get(skip, pageLimit);
        }
    }
}
