namespace MultiEditorPCC.Components.Pages;

using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

[Route("[controller]/[action]")]
public class LngController : Controller
{
    public IActionResult Set(string lng, string localRedirectUri)
    {
        if (lng != null)
        {
            HttpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(
                    new RequestCulture(lng, lng)));
        }

        return LocalRedirect(localRedirectUri);
    }
}



