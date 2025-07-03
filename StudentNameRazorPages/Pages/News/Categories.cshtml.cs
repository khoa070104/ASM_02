using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FUNewsManagement.Models;
using StudentNameRazorPages.Helpers;
using FUNewsManagement.Services;

namespace StudentNameRazorPages.Pages.News;

public class CategoriesModel : PageModel
{
    private readonly ICategoryService _categoryService;
    private readonly INewsArticleService _newsService;

    public CategoriesModel(ICategoryService categoryService, INewsArticleService newsService)
    {
        _categoryService = categoryService;
        _newsService = newsService;
    }

    public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    public Dictionary<short, int> CategoryNewsCounts { get; set; } = new Dictionary<short, int>();
    public string SearchTerm { get; set; } = string.Empty;
    public bool IsLecturer { get; set; }
    public bool IsStaff { get; set; }
    public bool IsAdmin { get; set; }

    public async Task<IActionResult> OnGetAsync(string? search)
    {
        // Check if user is logged in
        if (!SessionHelper.IsLoggedIn(HttpContext.Session))
        {
            return RedirectToPage("/Login");
        }

        IsLecturer = SessionHelper.IsLecturer(HttpContext.Session);
        IsStaff = SessionHelper.IsStaff(HttpContext.Session);
        IsAdmin = SessionHelper.IsAdmin(HttpContext.Session);

        SearchTerm = search ?? string.Empty;

        try
        {
            await LoadDataAsync();
            return Page();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error loading categories.";
            return Page();
        }
    }

    private async Task LoadDataAsync()
    {
        // Load active categories
        var allCategories = await _categoryService.GetActiveCategoriesAsync();

        // Apply search filter if provided
        if (!string.IsNullOrEmpty(SearchTerm))
        {
            allCategories = allCategories.Where(c => 
                c.CategoryName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (c.CategoryDescription != null && c.CategoryDescription.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)));
        }

        Categories = allCategories.OrderBy(c => c.CategoryName);

        // Get news counts for each category
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

        // Count news articles per category
        foreach (var category in Categories)
        {
            var newsCount = allNews.Count(n => n.CategoryId == category.CategoryId);
            CategoryNewsCounts[category.CategoryId] = newsCount;
        }
    }
} 