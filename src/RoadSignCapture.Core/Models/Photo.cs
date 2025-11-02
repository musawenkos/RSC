using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadSignCapture.Core.Models
{
    public class Photo
    {
        public Guid Id { get; set; }
        public Guid SignId { get; set; }
        public Sign Sign { get; set; }

        public string FilePath { get; set; }                
        public double Latitude { get; set; }                
        public double Longitude { get; set; }
        public string Description { get; set; }             
        public DateTime CapturedAt { get; set; }
        public string CapturedBy { get; set; }              
    }

}
