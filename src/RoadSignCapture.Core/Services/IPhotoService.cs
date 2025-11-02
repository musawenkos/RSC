using RoadSignCapture.Core.Models;
using RoadSignCapture.Core.Photos.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadSignCapture.Core.Services
{
    public interface IPhotoService
    {

        Task<IList<Photo>> GetAllPhotosAsync();

        Task<List<PhotoDto>> GetAllPhotosSignAsync(Guid signId);
        Task AddAsync(Photo photo);
        Task SaveChangesAsync();
        void Remove(Photo photo);
    }
}
