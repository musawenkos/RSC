using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RoadSignCapture.Web.Pages
{
    public class LogoutModel : PageModel
    {
        public async Task OnGet()
        {
            var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
                .WithRedirectUri("/RoadSignCapture/")
                .Build();

            await HttpContext.SignOutAsync(
                Auth0Constants.AuthenticationScheme,
                authenticationProperties);

            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
