using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FUNewsManagement.Models;
using FUNewsManagement.Services;

namespace StudentNameRazorPages.Pages.News;

public class PublicDetailsModel : PageModel
{
    private readonly INewsArticleService _newsService;

    public PublicDetailsModel(INewsArticleService newsService)
    {
        _newsService = newsService;
    }

    public NewsArticle? NewsArticle { get; set; }
    public IEnumerable<NewsArticle> RelatedNews { get; set; } = new List<NewsArticle>();

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        try
        {
            NewsArticle = await _newsService.GetNewsByIdAsync(id);

            if (NewsArticle == null || !NewsArticle.NewsStatus)
            {
                // Only show active news to public
                return NotFound();
            }

            // Load related news from the same category (only active news)
            var categoryNews = await _newsService.GetNewsByCategoryAsync(NewsArticle.CategoryId);
            RelatedNews = categoryNews
                .Where(n => n.NewsArticleId != id && n.NewsStatus) // Exclude current news and only active
                .OrderByDescending(n => n.CreatedDate)
                .Take(5);

            return Page();
        }
        catch (Exception ex)
        {
            // Log error but don't expose to public users
            return NotFound();
        }
    }
}
