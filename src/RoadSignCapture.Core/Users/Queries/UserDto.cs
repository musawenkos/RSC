using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadSignCapture.Core.Users.Queries
{
    public class UserDto
    {
        public string Email { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
}
