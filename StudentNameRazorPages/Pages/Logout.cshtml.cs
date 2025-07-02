using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentNameRazorPages.Helpers;

namespace StudentNameRazorPages.Pages;

public class LogoutModel : PageModel
{
    public IActionResult OnGet()
    {
        SessionHelper.ClearCurrentUser(HttpContext.Session);
        return RedirectToPage("/Login");
    }
}
