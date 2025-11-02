using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoadSignCapture.Core.Models;
using RoadSignCapture.Core.Services;
using RoadSignCapture.Core.Signs.Queries;
using RoadSignCapture.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadSignCapture.Infrastructure.Services
{
    public class SignService : ISignService
    {
        private readonly RSCDbContext? _context;

        private readonly ILogger<SignService> _logger;
        private readonly IPhotoService _photoService;
        private readonly IAudiTrailService _audiTrailService;

        public SignService(RSCDbContext? context, ILogger<SignService> logger, IPhotoService photoService, IAudiTrailService audiTrailService)
        {
            _context = context;
            _logger = logger;
            _photoService = photoService;
            _audiTrailService = audiTrailService;
        }

        public async Task<List<SignDto?>> GetListSignAsync(string projectName)
        {
            List<SignDto>? signDtoList = new List<SignDto>();
            try
            {
                var signs = await _context!.Signs
                    .AsNoTracking()
                    .Where(s => s.ProjectId == projectName)
                    .ToListAsync();

                foreach (var sign in signs)
                {
                    var photos = await _photoService.GetAllPhotosSignAsync(sign.Id);
                    var signAudits = await _audiTrailService.GetAllAuditTrailsSignAsync(sign.Id);
                    var signDto = new SignDto
                    {
                        Id = sign.Id,
                        SignIdNumber = sign.SignIdNumber,
                        SARTSMCode = sign.SARTSMCode,
                        SignType = sign.SignType,
                        RouteName = sign.RouteName,
                        NodeNumber = sign.NodeNumber,
                        Action = sign.Action,

                        WidthMm = sign.WidthMm,
                        HeightMm = sign.HeightMm,
                        AreaM2 = sign.AreaM2,
                        NumPoles = sign.NumPoles,
                        PoleDiameter = sign.PoleDiameter,
                        PoleLengthMm = sign.PoleLengthMm,
                        ExcavationDepthCubicM = sign.ExcavationDepthCubicM,
                        SupportType = sign.SupportType,

                        Latitude = sign.Latitude,
                        Longitude = sign.Longitude,
                        OffsetDistanceM = sign.OffsetDistanceM,
                        InstallationStatus = sign.InstallationStatus,

                        Photos = [.. photos],
                        Audits = [.. signAudits]

                    };
                    signDtoList!.Add(signDto);
                }
                if (signDtoList != null) { 
                    return signDtoList!;
                }
                return [];
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: Can't get list of signs because: {ex}",ex.Message);
                return [];
            }
        }

        public async Task AddAsync(Sign sign) => await _context!.Signs.AddAsync(sign);
        public void Remove(Sign sign) => _context!.Signs.Remove(sign);
        public async Task SaveChangesAsync() => await _context!.SaveChangesAsync();
    }
}
