using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using RoadSignCapture.Core.Models;

namespace RoadSignCapture.Core.Companies.Commands
{
    public class CompanyCommands
    {
        public int CompanyId { get; set; }
        public required string CompanyName { get; set; }
        public string? FullAddress { get; set; }
        public string? ContactNumber { get; set; }
    }
}
