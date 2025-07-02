using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FUNewsManagement.Models;
using StudentNameRazorPages.Helpers;
using FUNewsManagement.Services;

namespace StudentNameRazorPages.Pages.Staff.News;

public class IndexModel : PageModel
{
    private readonly INewsArticleService _newsService;

    public IndexModel(INewsArticleService newsService)
    {
        _newsService = newsService;
    }

    public IEnumerable<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
    public string SearchTerm { get; set; } = string.Empty;
    public bool? StatusFilter { get; set; }

    public async Task<IActionResult> OnGetAsync(string? search, bool? status)
    {
        // Check if user is staff
        if (!SessionHelper.IsStaff(HttpContext.Session))
        {
            return RedirectToPage("/Login");
        }

        var currentUser = SessionHelper.GetCurrentUser(HttpContext.Session);
        if (currentUser == null)
        {
            return RedirectToPage("/Login");
        }

        SearchTerm = search ?? string.Empty;
        StatusFilter = status;

        try
        {
            // Load user's news articles
            var myNews = await _newsService.GetNewsByAuthorAsync(currentUser.AccountId);

            // Apply filters
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                myNews = myNews.Where(n => 
                    n.NewsTitle.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (n.NewsContent != null && n.NewsContent.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (n.Headline != null && n.Headline.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)));
            }

            if (StatusFilter.HasValue)
            {
                myNews = myNews.Where(n => n.NewsStatus == StatusFilter.Value);
            }

            NewsArticles = myNews.OrderByDescending(n => n.CreatedDate);

            return Page();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error loading your news articles.";
            return Page();
        }
    }
}
