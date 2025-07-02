using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FUNewsManagement.Models;
using StudentNameRazorPages.Helpers;
using FUNewsManagement.Services;

namespace StudentNameRazorPages.Pages.News;

public class IndexModel : PageModel
{
    private readonly INewsArticleService _newsService;
    private readonly ICategoryService _categoryService;

    public IndexModel(
        INewsArticleService newsService,
        ICategoryService categoryService)
    {
        _newsService = newsService;
        _categoryService = categoryService;
    }

    public IEnumerable<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
    public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    public SystemAccount? CurrentUser { get; set; }
    public bool IsStaff { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsLecturer { get; set; }
    public string SearchTerm { get; set; } = string.Empty;
    public short? CategoryId { get; set; }

    public async Task<IActionResult> OnGetAsync(string? search, short? categoryId)
    {
        // Check if user is logged in
        if (!SessionHelper.IsLoggedIn(HttpContext.Session))
        {
            return RedirectToPage("/Login");
        }

        CurrentUser = SessionHelper.GetCurrentUser(HttpContext.Session);
        IsStaff = SessionHelper.IsStaff(HttpContext.Session);
        IsAdmin = SessionHelper.IsAdmin(HttpContext.Session);
        IsLecturer = SessionHelper.IsLecturer(HttpContext.Session);

        SearchTerm = search ?? string.Empty;
        CategoryId = categoryId;

        try
        {
            // Load categories for filter
            Categories = await _categoryService.GetActiveCategoriesAsync();

            // Load news based on user role and filters
            IEnumerable<NewsArticle> allNews;

            if (IsLecturer)
            {
                // Lecturers can only see active news
                allNews = await _newsService.GetActiveNewsAsync();
            }
            else
            {
                // Staff and Admin can see all news
                allNews = await _newsService.GetAllNewsAsync();
            }

            // Apply filters
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                allNews = await _newsService.SearchNewsAsync(SearchTerm);
            }

            if (CategoryId.HasValue)
            {
                allNews = allNews.Where(n => n.CategoryId == CategoryId.Value);
            }

            NewsArticles = allNews.OrderByDescending(n => n.CreatedDate);

            return Page();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error loading news articles.";
            return Page();
        }
    }
}
