using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FUNewsManagement.Models;
using StudentNameRazorPages.Helpers;
using FUNewsManagement.Services;

namespace StudentNameRazorPages.Pages.Staff.News;

public class EditModel : PageModel
{
    private readonly INewsArticleService _newsService;
    private readonly ICategoryService _categoryService;
    private readonly ITagService _tagService;

    public EditModel(
        INewsArticleService newsService,
        ICategoryService categoryService,
        ITagService tagService)
    {
        _newsService = newsService;
        _categoryService = categoryService;
        _tagService = tagService;
    }

    [BindProperty]
    public NewsArticle NewsArticle { get; set; } = new();

    [BindProperty]
    public List<int> SelectedTagIds { get; set; } = new();

    public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    public IEnumerable<Tag> Tags { get; set; } = new List<Tag>();

    public async Task<IActionResult> OnGetAsync(string id)
    {
        // Check if user is staff
        if (!SessionHelper.IsStaff(HttpContext.Session))
        {
            return RedirectToPage("/Login");
        }

        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        try
        {
            NewsArticle = await _newsService.GetNewsByIdAsync(id);
            if (NewsArticle == null)
            {
                TempData["ErrorMessage"] = "News article not found.";
                return RedirectToPage("/Staff/News/Index");
            }

            // Check if current user can edit this news (only creator or admin)
            var currentUserId = SessionHelper.GetUserId(HttpContext.Session);
            var currentUserRole = SessionHelper.GetUserRole(HttpContext.Session);

            if (currentUserRole != 0 && NewsArticle.CreatedById != currentUserId) // Not admin and not creator
            {
                TempData["ErrorMessage"] = "You can only edit news articles that you created.";
                return RedirectToPage("/Staff/News/Index");
            }

            await LoadDataAsync();

            // Load selected tags
            SelectedTagIds = NewsArticle.NewsTags.Select(nt => nt.TagId).ToList();

            return Page();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error loading news article.";
            return RedirectToPage("/Staff/News/Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Check if user is staff
        if (!SessionHelper.IsStaff(HttpContext.Session))
        {
            return RedirectToPage("/Login");
        }

        try
        {
            // Validate model
            if (!ModelState.IsValid)
            {
                await LoadDataAsync();
                return Page();
            }

            // Check if news exists and user can edit
            var existingNews = await _newsService.GetNewsByIdAsync(NewsArticle.NewsArticleId);
            if (existingNews == null)
            {
                TempData["ErrorMessage"] = "News article not found.";
                return RedirectToPage("/Staff/News/Index");
            }

            var currentUserId = SessionHelper.GetUserId(HttpContext.Session);
            var currentUserRole = SessionHelper.GetUserRole(HttpContext.Session);

            if (currentUserRole != 0 && existingNews.CreatedById != currentUserId)
            {
                TempData["ErrorMessage"] = "You can only edit news articles that you created.";
                return RedirectToPage("/Staff/News/Index");
            }

            // Update news article
            existingNews.NewsTitle = NewsArticle.NewsTitle;
            existingNews.Headline = NewsArticle.Headline;
            existingNews.NewsContent = NewsArticle.NewsContent;
            existingNews.NewsSource = NewsArticle.NewsSource;
            existingNews.CategoryId = NewsArticle.CategoryId;
            existingNews.NewsStatus = NewsArticle.NewsStatus;
            existingNews.ModifiedDate = DateTime.Now;

            // Update news with tags
            await _newsService.UpdateNewsAsync(existingNews, SelectedTagIds);

            TempData["SuccessMessage"] = "News article updated successfully!";
            return RedirectToPage("/Staff/News/Index");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error updating news article. Please try again.";
            await LoadDataAsync();
            return Page();
        }
    }

    private async Task LoadDataAsync()
    {
        Categories = await _categoryService.GetActiveCategoriesAsync();
        Tags = await _tagService.GetAllTagsAsync();
    }
}
