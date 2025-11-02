using RoadSignCapture.Core.Models;
using RoadSignCapture.Core.Projects.Queries;
using RoadSignCapture.Core.Signs.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadSignCapture.Core.Services
{
    public interface ISignService
    {
        Task<List<SignDto?>> GetListSignAsync(string projectName);
        Task AddAsync(Sign sign);
        Task SaveChangesAsync();
        void Remove(Sign sign);
    }
}
