using RoadSignCapture.Core.Models;
using RoadSignCapture.Core.SignAuditTrail.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadSignCapture.Core.Services
{
    public interface IAudiTrailService
    {
        Task<IList<AuditTrail>> GetAllAuditTrailsAsync();
        Task<List<AuditTrailDto>> GetAllAuditTrailsSignAsync(Guid signId);
        Task AddAsync(AuditTrail auditTrail);
        Task SaveChangesAsync();
        void Remove(AuditTrail auditTrail);
    }
}
