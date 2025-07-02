using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FUNewsManagement.Models;
using StudentNameRazorPages.Helpers;
using FUNewsManagement.Services;

namespace StudentNameRazorPages.Pages.Staff.Profile;

public class IndexModel : PageModel
{
    private readonly ISystemAccountService _accountService;
    private readonly INewsArticleService _newsService;

    public IndexModel(
        ISystemAccountService accountService,
        INewsArticleService newsService)
    {
        _accountService = accountService;
        _newsService = newsService;
    }

    public SystemAccount? Account { get; set; }
    public int TotalNewsCreated { get; set; }
    public int ActiveNewsCount { get; set; }
    public int CategoriesUsed { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        // Check if user is staff or lecturer
        if (!SessionHelper.IsStaff(HttpContext.Session))
        {
            return RedirectToPage("/Login");
        }

        try
        {
            var currentUserId = SessionHelper.GetUserId(HttpContext.Session);
            Account = await _accountService.GetAccountByIdAsync(currentUserId);

            if (Account == null)
            {
                TempData["ErrorMessage"] = "Account not found.";
                return RedirectToPage("/Login");
            }

            // Load statistics for staff
            if (Account.AccountRole == 1) // Staff
            {
                await LoadStaffStatisticsAsync(currentUserId);
            }

            return Page();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error loading profile.";
            return Page();
        }
    }

    public async Task<IActionResult> OnPostUpdateProfileAsync(string accountName, string accountEmail)
    {
        // Check if user is staff or lecturer
        if (!SessionHelper.IsStaff(HttpContext.Session))
        {
            return RedirectToPage("/Login");
        }

        try
        {
            var currentUserId = SessionHelper.GetUserId(HttpContext.Session);
            Account = await _accountService.GetAccountByIdAsync(currentUserId);

            if (Account == null)
            {
                TempData["ErrorMessage"] = "Account not found.";
                return RedirectToPage();
            }

            // Validate input
            if (string.IsNullOrWhiteSpace(accountName) || string.IsNullOrWhiteSpace(accountEmail))
            {
                TempData["ErrorMessage"] = "Please fill in all required fields.";
                return RedirectToPage();
            }

            // Check if email already exists (excluding current account)
            var existingAccount = await _accountService.GetAccountByEmailAsync(accountEmail);
            if (existingAccount != null && existingAccount.AccountId != currentUserId)
            {
                TempData["ErrorMessage"] = "An account with this email already exists.";
                return RedirectToPage();
            }

            // Update account
            Account.AccountName = accountName;
            Account.AccountEmail = accountEmail;

            await _accountService.UpdateAccountAsync(Account);

            // Update session with new name
            HttpContext.Session.SetString("UserName", Account.AccountName);

            TempData["SuccessMessage"] = "Profile updated successfully!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error updating profile. Please try again.";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostChangePasswordAsync(string currentPassword, string newPassword, string confirmPassword)
    {
        // Check if user is staff or lecturer
        if (!SessionHelper.IsStaff(HttpContext.Session))
        {
            return RedirectToPage("/Login");
        }

        try
        {
            var currentUserId = SessionHelper.GetUserId(HttpContext.Session);
            Account = await _accountService.GetAccountByIdAsync(currentUserId);

            if (Account == null)
            {
                TempData["ErrorMessage"] = "Account not found.";
                return RedirectToPage();
            }

            // Validate input
            if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                TempData["ErrorMessage"] = "Please fill in all password fields.";
                return RedirectToPage();
            }

            if (newPassword != confirmPassword)
            {
                TempData["ErrorMessage"] = "New password and confirmation do not match.";
                return RedirectToPage();
            }

            if (newPassword.Length < 6)
            {
                TempData["ErrorMessage"] = "New password must be at least 6 characters long.";
                return RedirectToPage();
            }

            // Verify current password
            if (Account.AccountPassword != currentPassword) // In real app, should hash and compare
            {
                TempData["ErrorMessage"] = "Current password is incorrect.";
                return RedirectToPage();
            }

            // Update password
            Account.AccountPassword = newPassword; // In real app, should hash this

            await _accountService.UpdateAccountAsync(Account);

            TempData["SuccessMessage"] = "Password changed successfully!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error changing password. Please try again.";
        }

        return RedirectToPage();
    }

    private async Task LoadStaffStatisticsAsync(short userId)
    {
        try
        {
            var allNews = await _newsService.GetAllNewsAsync();
            var userNews = allNews.Where(n => n.CreatedById == userId);

            TotalNewsCreated = userNews.Count();
            ActiveNewsCount = userNews.Count(n => n.NewsStatus);
            CategoriesUsed = userNews.Select(n => n.CategoryId).Distinct().Count();
        }
        catch (Exception ex)
        {
            // If statistics fail to load, set to 0
            TotalNewsCreated = 0;
            ActiveNewsCount = 0;
            CategoriesUsed = 0;
        }
    }
}
