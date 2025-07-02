using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FUNewsManagement.Models;
using StudentNameRazorPages.Helpers;
using FUNewsManagement.Services;

namespace StudentNameRazorPages.Pages.Admin.Reports;

public class IndexModel : PageModel
{
    private readonly INewsArticleService _newsService;
    private readonly ISystemAccountService _accountService;

    public IndexModel(
        INewsArticleService newsService,
        ISystemAccountService accountService)
    {
        _newsService = newsService;
        _accountService = accountService;
    }

    [BindProperty(SupportsGet = true)]
    public DateTime? StartDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? EndDate { get; set; }

    public IEnumerable<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
    public int TotalNews { get; set; }
    public int ActiveNews { get; set; }
    public int InactiveNews { get; set; }
    public int TotalAuthors { get; set; }
    public IEnumerable<CategoryStatistic> CategoryStats { get; set; } = new List<CategoryStatistic>();

    public class CategoryStatistic
    {
        public string CategoryName { get; set; } = string.Empty;
        public int NewsCount { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        // Check if user is admin
        if (!SessionHelper.IsAdmin(HttpContext.Session))
        {
            return RedirectToPage("/Login");
        }

        try
        {
            // Load all news
            var allNews = await _newsService.GetAllNewsAsync();

            // Apply date filter if provided
            if (StartDate.HasValue || EndDate.HasValue)
            {
                if (StartDate.HasValue && EndDate.HasValue)
                {
                    if (StartDate > EndDate)
                    {
                        TempData["ErrorMessage"] = "Start date cannot be greater than end date.";
                        return Page();
                    }
                    allNews = allNews.Where(n => n.CreatedDate.Date >= StartDate.Value.Date && 
                                                n.CreatedDate.Date <= EndDate.Value.Date);
                }
                else if (StartDate.HasValue)
                {
                    allNews = allNews.Where(n => n.CreatedDate.Date >= StartDate.Value.Date);
                }
                else if (EndDate.HasValue)
                {
                    allNews = allNews.Where(n => n.CreatedDate.Date <= EndDate.Value.Date);
                }
            }

            // Sort by created date descending as required
            NewsArticles = allNews.OrderByDescending(n => n.CreatedDate);

            // Calculate statistics
            TotalNews = NewsArticles.Count();
            ActiveNews = NewsArticles.Count(n => n.NewsStatus);
            InactiveNews = NewsArticles.Count(n => !n.NewsStatus);
            TotalAuthors = NewsArticles.Select(n => n.CreatedById).Distinct().Count();

            // Category statistics
            CategoryStats = NewsArticles
                .GroupBy(n => n.Category?.CategoryName ?? "Unknown")
                .Select(g => new CategoryStatistic
                {
                    CategoryName = g.Key,
                    NewsCount = g.Count()
                })
                .OrderByDescending(s => s.NewsCount);

            return Page();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error loading report data.";
            return Page();
        }
    }
}
