using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentNameRazorPages.Helpers;

namespace StudentNameRazorPages.Pages;

public class LogoutModel : PageModel
{
    public IActionResult OnGet()
    {
        try
        {
            // Clear all session data immediately
            HttpContext.Session.Clear();
            
            // Clear user session using helper
            SessionHelper.ClearCurrentUser(HttpContext.Session);
            
            // Clear all cookies
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie, new CookieOptions()
                {
                    Path = "/",
                    HttpOnly = true,
                    Secure = false, // Set to false for development
                    SameSite = SameSiteMode.Lax
                });
            }

            // Add cache control headers to prevent browser caching
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            // Commit session changes
            HttpContext.Session.CommitAsync().Wait();
        }
        catch (Exception)
        {
            // Even if there's an error, continue with logout
        }

        // Redirect to home page
        return RedirectToPage("/Index");
    }
}
