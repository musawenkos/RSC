using Microsoft.AspNetCore.Http;
using RoadSignCapture.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadSignCapture.Core.Signs.Commands
{
    public class SignCommand
    {
        public required Sign? Sign { get; set; }
        public IFormFile? PdfFile { get; set; }
        public IFormFile? JpgFile { get; set; }
        public IFormFile? SgxFile { get; set; }
    }
}
