using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FUNewsManagement.Models;
using StudentNameRazorPages.Helpers;
using FUNewsManagement.Services;
using Microsoft.AspNetCore.SignalR;
using StudentNameRazorPages.Hubs;

namespace StudentNameRazorPages.Pages.Staff.News;

public class DeleteModel : PageModel
{
    private readonly INewsArticleService _newsService;
    private readonly IHubContext<NewsHub> _hubContext;

    public DeleteModel(INewsArticleService newsService, IHubContext<NewsHub> hubContext)
    {
        _newsService = newsService;
        _hubContext = hubContext;
    }

    [BindProperty]
    public NewsArticle NewsArticle { get; set; } = new();

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

            // Check if current user can delete this news (only creator or admin)
            var currentUserId = SessionHelper.GetUserId(HttpContext.Session);
            var currentUserRole = SessionHelper.GetUserRole(HttpContext.Session);
            
            if (currentUserRole != 0 && NewsArticle.CreatedById != currentUserId) // Not admin and not creator
            {
                TempData["ErrorMessage"] = "You can only delete news articles that you created.";
                return RedirectToPage("/Staff/News/Index");
            }

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
            // Check if news exists and user can delete
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
                TempData["ErrorMessage"] = "You can only delete news articles that you created.";
                return RedirectToPage("/Staff/News/Index");
            }

            // Delete news article
            var success = await _newsService.DeleteNewsAsync(NewsArticle.NewsArticleId);
            
            if (success)
            {
                // Send real-time notification
                await _hubContext.Clients.All.SendAsync("NewsDeleted", NewsArticle.NewsArticleId);
                
                TempData["SuccessMessage"] = "News article deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete news article.";
            }

            return RedirectToPage("/Staff/News/Index");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error deleting news article. Please try again.";
            return RedirectToPage("/Staff/News/Index");
        }
    }
}
