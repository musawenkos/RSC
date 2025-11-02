using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadSignCapture.Core.SignAuditTrail.Queries
{
    public class AuditTrailDto
    {
        public Guid Id { get; set; }
        public Guid SignId { get; set; }
        public string ActionType { get; set; }
        public string Description { get; set; }
        public string PerformedBy { get; set; }
        public DateTime Timestamp { get; set; }
        public string DeviceUsed { get; set; }
        public string LocationContext { get; set; }
    }
}
