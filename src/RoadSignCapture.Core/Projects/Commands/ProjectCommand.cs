using RoadSignCapture.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadSignCapture.Core.Projects.Commands
{
    public class ProjectCommand
    {
        public required string? ProjectName{ get; set; }
        public string? ProjectDescription{ get; set; }
        public required List<string> SelectedUsers { get; set; }
    }
}
