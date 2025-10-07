using RoadSignCapture.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadSignCapture.Core.Users.Commands
{
    public class CreateUserCommand
    {
        public required User? Users { get; set; }
        public int SelectedRoleId { get; set; } = 0;
    }
}
