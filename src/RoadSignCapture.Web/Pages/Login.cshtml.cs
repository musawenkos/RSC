using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RoadSignCapture.Web.Pages
{
    public class LoginModel : PageModel
    {
        public async Task OnGet(string returnUrl = "/RoadSignCapture/")
        {
            var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
                .WithRedirectUri(returnUrl)
                .Build();

            await HttpContext.ChallengeAsync(
                Auth0Constants.AuthenticationScheme,
                authenticationProperties);
        }
    }
}
