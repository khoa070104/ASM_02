using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FUNewsManagement.Models;
using StudentNameRazorPages.Helpers;
using FUNewsManagement.Services;

namespace StudentNameRazorPages.Pages.Admin;

public class DashboardModel : PageModel
{
    private readonly ISystemAccountService _accountService;
    private readonly INewsArticleService _newsService;
    private readonly ICategoryService _categoryService;
    private readonly ITagService _tagService;

    public DashboardModel(
        ISystemAccountService accountService,
        INewsArticleService newsService,
        ICategoryService categoryService,
        ITagService tagService)
    {
        _accountService = accountService;
        _newsService = newsService;
        _categoryService = categoryService;
        _tagService = tagService;
    }

    public int TotalAccounts { get; set; }
    public int TotalNews { get; set; }
    public int TotalCategories { get; set; }
    public int TotalTags { get; set; }
    public IEnumerable<NewsArticle> RecentNews { get; set; } = new List<NewsArticle>();

    public async Task<IActionResult> OnGetAsync()
    {
        // Check if user is admin
        if (!SessionHelper.IsAdmin(HttpContext.Session))
        {
            return RedirectToPage("/Login");
        }

        try
        {
            // Load dashboard data
            var accounts = await _accountService.GetAllAccountsAsync();
            var news = await _newsService.GetAllNewsAsync();
            var categories = await _categoryService.GetAllCategoriesAsync();
            var tags = await _tagService.GetAllTagsAsync();

            TotalAccounts = accounts.Count();
            TotalNews = news.Count();
            TotalCategories = categories.Count();
            TotalTags = tags.Count();
            RecentNews = news.OrderByDescending(n => n.CreatedDate).Take(5);

            return Page();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error loading dashboard data.";
            return Page();
        }
    }
}
