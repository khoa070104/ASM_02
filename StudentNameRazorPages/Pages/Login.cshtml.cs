using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentNameRazorPages.Helpers;
using FUNewsManagement.Services;

namespace StudentNameRazorPages.Pages;

public class LoginModel : PageModel
{
    private readonly ISystemAccountService _accountService;

    public LoginModel(ISystemAccountService accountService)
    {
        _accountService = accountService;
    }

    [BindProperty]
    public Models.LoginModel Input { get; set; } = new();

    public IActionResult OnGet()
    {
        // If user is already logged in, redirect to appropriate dashboard
        if (SessionHelper.IsLoggedIn(HttpContext.Session))
        {
            var user = SessionHelper.GetCurrentUser(HttpContext.Session);
            return RedirectToPage(GetDashboardPage(user?.AccountRole ?? 2));
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var user = await _accountService.AuthenticateAsync(Input.Email, Input.Password);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Invalid email or password.";
                return Page();
            }

            // Set user session
            SessionHelper.SetCurrentUser(HttpContext.Session, user);

            // Redirect based on user role
            return RedirectToPage(GetDashboardPage(user.AccountRole));
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "An error occurred during login. Please try again.";
            return Page();
        }
    }

    private string GetDashboardPage(int role)
    {
        return role switch
        {
            0 => "/Admin/Dashboard", // Admin
            1 => "/Staff/Dashboard",  // Staff
            2 => "/Lecturer/Dashboard", // Lecturer
            _ => "/News/Public" // Default to public news
        };
    }
}
