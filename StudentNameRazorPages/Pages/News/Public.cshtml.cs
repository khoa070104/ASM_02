using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FUNewsManagement.Models;
using FUNewsManagement.Services;

namespace StudentNameRazorPages.Pages.News;

public class PublicModel : PageModel
{
    private readonly INewsArticleService _newsService;
    private readonly ICategoryService _categoryService;

    public PublicModel(
        INewsArticleService newsService,
        ICategoryService categoryService)
    {
        _newsService = newsService;
        _categoryService = categoryService;
    }

    public IEnumerable<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
    public IEnumerable<NewsArticle> FeaturedNews { get; set; } = new List<NewsArticle>();
    public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    public string SearchTerm { get; set; } = string.Empty;
    public short? CategoryId { get; set; }

    public async Task<IActionResult> OnGetAsync(string? search, short? categoryId)
    {
        SearchTerm = search ?? string.Empty;
        CategoryId = categoryId;

        try
        {
            // Load categories for filter
            Categories = await _categoryService.GetActiveCategoriesAsync();

            // Load only active news for public viewing
            var allNews = await _newsService.GetActiveNewsAsync();

            // Apply filters
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                allNews = allNews.Where(n => 
                    n.NewsTitle.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (n.NewsContent != null && n.NewsContent.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (n.Headline != null && n.Headline.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)));
            }

            if (CategoryId.HasValue)
            {
                allNews = allNews.Where(n => n.CategoryId == CategoryId.Value);
            }

            // Get featured news (latest 3 news)
            FeaturedNews = allNews.OrderByDescending(n => n.CreatedDate).Take(3);

            // Get all news ordered by date
            NewsArticles = allNews.OrderByDescending(n => n.CreatedDate);

            return Page();
        }
        catch (Exception ex)
        {
            // Log error but don't show to public users
            NewsArticles = new List<NewsArticle>();
            FeaturedNews = new List<NewsArticle>();
            Categories = new List<Category>();
            return Page();
        }
    }
}
