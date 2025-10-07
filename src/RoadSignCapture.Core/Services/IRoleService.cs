using RoadSignCapture.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadSignCapture.Core.Services
{
    public interface IRoleService
    {
        Task<Role?> GetByIdAsync(int RoleId);
        Task<List<Role>> GetAllAsync();
    }
}
