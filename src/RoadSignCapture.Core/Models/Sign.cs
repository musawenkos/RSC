using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadSignCapture.Core.Models
{
    public class Sign
    {
        public Guid Id { get; set; }                        
        public string SignIdNumber { get; set; }            
        public string SignType { get; set; }                
        public string SARTSMCode { get; set; }              
        public string RouteName { get; set; }               
        public string NodeNumber { get; set; }              
        public string Action { get; set; }                  

        
        public double WidthMm { get; set; }
        public double HeightMm { get; set; }
        public double AreaM2 { get; set; }
        public int NumPoles { get; set; }
        public string PoleDiameter { get; set; }            
        public double PoleLengthMm { get; set; }
        public double ExcavationDepthCubicM { get; set; } // Excavation Depth m3
        public string SupportType { get; set; }             

        
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double OffsetDistanceM { get; set; }         
        public string InstallationStatus { get; set; }

        
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }



        public string? CreatedByUserId { get; set; }
        public User? CreatedByUser { get; set; }

        public string? ProjectId { get; set; }
        public Project? Project { get; set; }

        public string? ClientId { get; set; }
        public User? Client { get; set; }

        public ICollection<Photo> Photos { get; set; }
        public ICollection<AuditTrail> AuditTrails { get; set; }
    }

}
