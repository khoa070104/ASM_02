using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FUNewsManagement.Models;
using StudentNameRazorPages.Helpers;
using FUNewsManagement.Services;

namespace StudentNameRazorPages.Pages.Staff;

public class DashboardModel : PageModel
{
    private readonly INewsArticleService _newsService;
    private readonly ICategoryService _categoryService;

    public DashboardModel(
        INewsArticleService newsService,
        ICategoryService categoryService)
    {
        _newsService = newsService;
        _categoryService = categoryService;
    }

    public SystemAccount? CurrentUser { get; set; }
    public int MyNewsCount { get; set; }
    public int ActiveNewsCount { get; set; }
    public int TotalCategories { get; set; }
    public IEnumerable<NewsArticle> MyRecentNews { get; set; } = new List<NewsArticle>();

    public async Task<IActionResult> OnGetAsync()
    {
        // Check if user is staff
        if (!SessionHelper.IsStaff(HttpContext.Session))
        {
            return RedirectToPage("/Login");
        }

        CurrentUser = SessionHelper.GetCurrentUser(HttpContext.Session);
        if (CurrentUser == null)
        {
            return RedirectToPage("/Login");
        }

        try
        {
            // Load dashboard data
            var myNews = await _newsService.GetNewsByAuthorAsync(CurrentUser.AccountId);
            var categories = await _categoryService.GetActiveCategoriesAsync();

            MyNewsCount = myNews.Count();
            ActiveNewsCount = myNews.Count(n => n.NewsStatus);
            TotalCategories = categories.Count();
            MyRecentNews = myNews.OrderByDescending(n => n.CreatedDate).Take(5);

            return Page();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error loading dashboard data.";
            return Page();
        }
    }
}
