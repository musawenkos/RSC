using RoadSignCapture.Core.Models;
using RoadSignCapture.Core.Users.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadSignCapture.Core.Services
{
    public interface IUserService
    {
        Task<IList<User>> GetAllUsersAsync(string authUserEmail);

        Task<IList<User>> GetAllUsersAsync();
        Task<UserDto?> GetUserDetailsBy(string email);
        Task<UserDto> GetClientFrom(List<User> users);
        Task AddAsync(User user);
        Task SaveChangesAsync();
        Task<User?> GetUserBy(string email);
        bool UserExists(string email);
        void Remove(User user);
        
    }
}
