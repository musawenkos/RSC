using RoadSignCapture.Core.Models;
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
        Task AddAsync(User user);
        Task SaveChangesAsync();
        Task<User?> GetUserBy(string id);
        bool UserExists(string email);
        void Remove(User user);
    }
}
