using RoadSignCapture.Core.Models;
using RoadSignCapture.Core.Users.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadSignCapture.Core.Projects.Queries
{
    public class ProjectDto
    {
        public string? ProjectName { get; set; }
        public string? ProjectDescription { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public List<UserDto>? Clients { get; set; }

    }
}
