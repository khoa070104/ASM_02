using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FUNewsManagement.Models;
using StudentNameRazorPages.Helpers;
using FUNewsManagement.Services;

namespace StudentNameRazorPages.Pages.News;

public class DetailsModel : PageModel
{
    private readonly INewsArticleService _newsService;

    public DetailsModel(INewsArticleService newsService)
    {
        _newsService = newsService;
    }

    public NewsArticle? NewsArticle { get; set; }
    public IEnumerable<NewsArticle> RelatedNews { get; set; } = new List<NewsArticle>();
    public SystemAccount? CurrentUser { get; set; }
    public bool IsStaff { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsLecturer { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        // Check if user is logged in
        if (!SessionHelper.IsLoggedIn(HttpContext.Session))
        {
            return RedirectToPage("/Login");
        }

        CurrentUser = SessionHelper.GetCurrentUser(HttpContext.Session);
        IsStaff = SessionHelper.IsStaff(HttpContext.Session);
        IsAdmin = SessionHelper.IsAdmin(HttpContext.Session);
        IsLecturer = SessionHelper.IsLecturer(HttpContext.Session);

        try
        {
            NewsArticle = await _newsService.GetNewsByIdAsync(id);

            if (NewsArticle == null)
            {
                return NotFound();
            }

            // Check if lecturer can view this news (must be active)
            if (IsLecturer && !NewsArticle.NewsStatus)
            {
                return NotFound();
            }

            // Load related news from the same category
            var categoryNews = await _newsService.GetNewsByCategoryAsync(NewsArticle.CategoryId);
            RelatedNews = categoryNews
                .Where(n => n.NewsArticleId != id && n.NewsStatus) // Exclude current news and only active
                .OrderByDescending(n => n.CreatedDate)
                .Take(5);

            return Page();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error loading news article.";
            return RedirectToPage("/News/Index");
        }
    }
}
