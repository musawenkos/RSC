using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph;
using RoadSignCapture.Core.Models;
using RoadSignCapture.Core.Services;
using System.Threading.Tasks;

namespace RoadSignCapture.Web.Pages.Users
{
    [Authorize(Policy = "RequireAdmin")]
    public class IndexModel : PageModel
    {
        private readonly IUserService? _userService;

        public IndexModel(IUserService userServ)
        {
            _userService = userServ;
        }
        public IList<Core.Models.User> Users { get; set; } = default!;
        public async Task OnGetAsync()
        {
            
            Users = await _userService!.GetAllUsersAsync();
        }
    }
}
