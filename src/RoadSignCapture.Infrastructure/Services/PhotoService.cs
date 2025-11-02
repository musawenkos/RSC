using Microsoft.EntityFrameworkCore;
using RoadSignCapture.Core.Models;
using RoadSignCapture.Core.Photos.Queries;
using RoadSignCapture.Core.Services;
using RoadSignCapture.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadSignCapture.Infrastructure.Services
{
    public class PhotoService : IPhotoService
    {
        public readonly RSCDbContext _context;

        public PhotoService(RSCDbContext context)
        {
            _context = context;
        }

        public async Task<IList<Photo>> GetAllPhotosAsync()
        {
            return await _context.Photos
                    .AsNoTracking()
                    .Include(s => s.Sign)
                    .ToListAsync();
        }

        public async Task<List<PhotoDto>> GetAllPhotosSignAsync(Guid signId)
        {
            List<PhotoDto> listPhotos = new List<PhotoDto>(); 
            var photos = await _context.Photos
                    .AsNoTracking()
                    .Include(s => s.Sign)
                    .Where(s => s.SignId == signId)
                    .ToListAsync();

            foreach (var photo in photos)
            {
                var photoDto = new PhotoDto
                { 
                    Id = photo.Id,
                    SignId = photo.SignId,
                    FilePath = photo.FilePath,
                    Latitude = photo.Latitude,
                    Longitude = photo.Longitude,
                    Description = photo.Description,
                };
                listPhotos.Add(photoDto);
            }
            return listPhotos;
        }
        public async Task AddAsync(Photo photo) => await _context.Photos.AddAsync(photo);
        public void Remove(Photo photo) => _context.Photos.Remove(photo);
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
