using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FUNewsManagement.Models;
using StudentNameRazorPages.Helpers;
using System.ComponentModel.DataAnnotations;
using FUNewsManagement.Services;

namespace StudentNameRazorPages.Pages.Staff.News;

public class CreateModel : PageModel
{
    private readonly INewsArticleService _newsService;
    private readonly ICategoryService _categoryService;
    private readonly ITagService _tagService;

    public CreateModel(
        INewsArticleService newsService,
        ICategoryService categoryService,
        ITagService tagService)
    {
        _newsService = newsService;
        _categoryService = categoryService;
        _tagService = tagService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [BindProperty]
    public List<int> SelectedTagIds { get; set; } = new();

    public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    public IEnumerable<Tag> Tags { get; set; } = new List<Tag>();

    public class InputModel
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string NewsTitle { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "Headline cannot exceed 250 characters")]
        public string? Headline { get; set; }

        [Required(ErrorMessage = "Content is required")]
        public string NewsContent { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Source cannot exceed 200 characters")]
        public string? NewsSource { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public short CategoryId { get; set; }

        public bool NewsStatus { get; set; } = true;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        // Check if user is staff
        if (!SessionHelper.IsStaff(HttpContext.Session))
        {
            return RedirectToPage("/Login");
        }

        await LoadDataAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
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

        if (!ModelState.IsValid)
        {
            await LoadDataAsync();
            return Page();
        }

        try
        {
            var newsArticle = new NewsArticle
            {
                NewsTitle = Input.NewsTitle,
                Headline = Input.Headline,
                NewsContent = Input.NewsContent,
                NewsSource = Input.NewsSource,
                CategoryId = Input.CategoryId,
                NewsStatus = Input.NewsStatus,
                CreatedById = currentUser.AccountID,
                CreatedDate = DateTime.Now
            };

            await _newsService.CreateNewsAsync(newsArticle, SelectedTagIds);

            TempData["SuccessMessage"] = "News article created successfully!";
            return RedirectToPage("/Staff/News/Index");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error creating news article. Please try again.";
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
