using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;
using System.Net;

namespace RoadSignCapture.Web.Pages
{
    [AuthorizeForScopes(ScopeKeySection = "MicrosoftGraph:Scopes")]
    public class IndexModel : PageModel
    {
        public int TotalProjects { get; set; }
        public int TotalSigns { get; set; }
        public List<SignViewModel> RecentSigns { get; set; } = new();
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
        public void OnGet()
        {
            // In real app: fetch from database via services
            TotalProjects = 3;
            TotalSigns = 128;

            RecentSigns = new List<SignViewModel>
            {
                new SignViewModel { SignId = "R23P6/2-4020-GCL", Type="Square", Location="Node 4020", ProjectName="R23 Upgrade", Status="Installed" },
                new SignViewModel { SignId = "R23P6/2-4020-BDE", Type="Round", Location="Node 4020", ProjectName="R23 Upgrade", Status="Installed" },
                new SignViewModel { SignId = "R23P6/2-4020-ZMF", Type="Triangular", Location="Node 4020", ProjectName="R23 Upgrade", Status="Planned" }
            };
        }

        public class SignViewModel
        {
            public string SignId { get; set; }
            public string Type { get; set; }
            public string Location { get; set; }
            public string ProjectName { get; set; }
            public string Status { get; set; }
        }
    }
}
