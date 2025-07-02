using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentNameRazorPages.Helpers;

namespace StudentNameRazorPages.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public bool IsAuthenticated { get; set; }

    public void OnGet()
    {
        IsAuthenticated = SessionHelper.IsLoggedIn(HttpContext.Session);
    }
}
