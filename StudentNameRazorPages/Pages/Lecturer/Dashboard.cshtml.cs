using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FUNewsManagement.Models;
using StudentNameRazorPages.Helpers;
using FUNewsManagement.Services;

namespace StudentNameRazorPages.Pages.Lecturer;

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
    public int TotalActiveNews { get; set; }
    public int TotalCategories { get; set; }
    public int RecentNewsCount { get; set; }
    public IEnumerable<NewsArticle> LatestNews { get; set; } = new List<NewsArticle>();
    public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    public string SearchTerm { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(string? search)
    {
        // Check if user is lecturer
        if (!SessionHelper.IsLecturer(HttpContext.Session))
        {
            return RedirectToPage("/Login");
        }

        CurrentUser = SessionHelper.GetCurrentUser(HttpContext.Session);
        if (CurrentUser == null)
        {
            return RedirectToPage("/Login");
        }

        SearchTerm = search ?? string.Empty;

        try
        {
            // Load dashboard data
            var activeNews = await _newsService.GetActiveNewsAsync();
            var categories = await _categoryService.GetActiveCategoriesAsync();

            TotalActiveNews = activeNews.Count();
            TotalCategories = categories.Count();
            
            // Recent news (this week)
            var weekAgo = DateTime.Now.AddDays(-7);
            RecentNewsCount = activeNews.Count(n => n.CreatedDate >= weekAgo);
            
            LatestNews = activeNews.OrderByDescending(n => n.CreatedDate).Take(10);
            Categories = categories.Take(6); // Show first 6 categories

            return Page();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error loading dashboard data.";
            return Page();
        }
    }
}
