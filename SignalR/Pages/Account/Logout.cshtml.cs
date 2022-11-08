using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SignalR.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGet()
        {
            HttpContext.Session.Remove("CustSession");
            HttpContext.Session.Remove("IsAdmin");
            HttpContext.Session.Remove("DeleteMessage");
            return Redirect("/Account/Login");
        }
    }
}
