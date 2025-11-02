using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadSignCapture.Core.Photos.Queries
{
    public class PhotoDto
    {
        public Guid Id { get; set; }
        public Guid SignId { get; set; }
        public string FilePath { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Description { get; set; }
    }
}
