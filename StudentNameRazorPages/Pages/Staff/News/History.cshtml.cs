using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FUNewsManagement.Models;
using StudentNameRazorPages.Helpers;
using FUNewsManagement.Services;

namespace StudentNameRazorPages.Pages.Staff.News;

public class HistoryModel : PageModel
{
    private readonly INewsArticleService _newsService;
    private readonly ICategoryService _categoryService;

    public HistoryModel(INewsArticleService newsService, ICategoryService categoryService)
    {
        _newsService = newsService;
        _categoryService = categoryService;
    }

    public IEnumerable<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
    public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    public string SearchTerm { get; set; } = string.Empty;
    public bool? StatusFilter { get; set; }
    public short? CategoryFilter { get; set; }

    // Statistics
    public int TotalNews { get; set; }
    public int PublishedNews { get; set; }
    public int DraftNews { get; set; }
    public int CategoriesUsed { get; set; }

    public async Task<IActionResult> OnGetAsync(string? search, bool? status, short? categoryId)
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
        CategoryFilter = categoryId;

        try
        {
            await LoadDataAsync(currentUser.AccountID);
            return Page();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error loading news history.";
            return Page();
        }
    }

    private async Task LoadDataAsync(short userId)
    {
        // Load all user's news
        var allUserNews = await _newsService.GetNewsByAuthorAsync(userId);

        // Calculate statistics
        TotalNews = allUserNews.Count();
        PublishedNews = allUserNews.Count(n => n.NewsStatus);
        DraftNews = allUserNews.Count(n => !n.NewsStatus);
        CategoriesUsed = allUserNews.Select(n => n.CategoryId).Distinct().Count();

        // Apply filters
        var filteredNews = allUserNews.AsEnumerable();

        if (!string.IsNullOrEmpty(SearchTerm))
        {
            filteredNews = filteredNews.Where(n => 
                n.NewsTitle.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (n.NewsContent != null && n.NewsContent.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (n.Headline != null && n.Headline.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)));
        }

        if (StatusFilter.HasValue)
        {
            filteredNews = filteredNews.Where(n => n.NewsStatus == StatusFilter.Value);
        }

        if (CategoryFilter.HasValue)
        {
            filteredNews = filteredNews.Where(n => n.CategoryId == CategoryFilter.Value);
        }

        NewsArticles = filteredNews.OrderByDescending(n => n.CreatedDate);

        // Load categories for filter dropdown
        Categories = await _categoryService.GetActiveCategoriesAsync();
    }
} 