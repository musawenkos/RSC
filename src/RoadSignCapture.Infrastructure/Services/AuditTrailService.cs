using Microsoft.EntityFrameworkCore;
using RoadSignCapture.Core.Models;
using RoadSignCapture.Core.Photos.Queries;
using RoadSignCapture.Core.Services;
using RoadSignCapture.Core.SignAuditTrail.Queries;
using RoadSignCapture.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadSignCapture.Infrastructure.Services
{
    public class AuditTrailService : IAudiTrailService
    {
        public readonly RSCDbContext _context;

        public AuditTrailService(RSCDbContext context)
        {  
            _context = context; 
        }

        public async Task<IList<AuditTrail>> GetAllAuditTrailsAsync()
        {
            return await _context.AuditTrails
                    .AsNoTracking()
                    .Include(s => s.Sign)
                    .ToListAsync();
        }

        public async Task<List<AuditTrailDto>> GetAllAuditTrailsSignAsync(Guid signId)
        {
            List<AuditTrailDto> listAudits = new List<AuditTrailDto>();
            var audits = await _context.AuditTrails
                    .AsNoTracking()
                    .Include(s => s.Sign)
                    .Where(s => s.SignId == signId)
                    .ToListAsync();

            foreach (var audit in audits)
            {
                var auditDto = new AuditTrailDto
                {
                    Id = audit.Id,
                    SignId = audit.SignId,
                    ActionType = audit.ActionType,
                    Description = audit.Description,
                    PerformedBy = audit.PerformedBy,
                    Timestamp = audit.Timestamp,
                    DeviceUsed = audit.DeviceUsed,
                    LocationContext = audit.LocationContext,
                };
                listAudits.Add(auditDto);
            }
            return listAudits;
        }


        public async Task AddAsync(AuditTrail auditTrail) => await _context.AuditTrails.AddAsync(auditTrail);
        public void Remove(AuditTrail auditTrail) => _context.AuditTrails.Remove(auditTrail);
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
