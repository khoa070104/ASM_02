using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FUNewsManagement.Models;
using StudentNameRazorPages.Helpers;
using FUNewsManagement.Services;

namespace StudentNameRazorPages.Pages.Admin.Accounts;

public class IndexModel : PageModel
{
    private readonly ISystemAccountService _accountService;

    public IndexModel(ISystemAccountService accountService)
    {
        _accountService = accountService;
    }

    public IEnumerable<SystemAccount> Accounts { get; set; } = new List<SystemAccount>();
    public string SearchTerm { get; set; } = string.Empty;
    public int? RoleFilter { get; set; }

    public async Task<IActionResult> OnGetAsync(string? search, int? role)
    {
        // Check if user is admin
        if (!SessionHelper.IsAdmin(HttpContext.Session))
        {
            return RedirectToPage("/Login");
        }

        SearchTerm = search ?? string.Empty;
        RoleFilter = role;

        try
        {
            await LoadDataAsync();
            return Page();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error loading accounts.";
            return Page();
        }
    }

    public async Task<IActionResult> OnPostCreateAsync(string accountName, string accountEmail, string accountPassword, short accountRole)
    {
        // Check if user is admin
        if (!SessionHelper.IsAdmin(HttpContext.Session))
        {
            return RedirectToPage("/Login");
        }

        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(accountName) || string.IsNullOrWhiteSpace(accountEmail) ||
                string.IsNullOrWhiteSpace(accountPassword) || accountRole < 1 || accountRole > 2)
            {
                TempData["ErrorMessage"] = "Please fill in all required fields with valid data.";
                return RedirectToPage();
            }

            // Check if email already exists
            var existingAccount = await _accountService.GetAccountByEmailAsync(accountEmail);
            if (existingAccount != null)
            {
                TempData["ErrorMessage"] = "An account with this email already exists.";
                return RedirectToPage();
            }

            var account = new SystemAccount
            {
                AccountName = accountName,
                AccountEmail = accountEmail,
                AccountPassword = accountPassword, // In real app, should hash this
                AccountRole = accountRole
            };

            var createdAccount = await _accountService.CreateAccountAsync(account);
            TempData["SuccessMessage"] = "Account created successfully!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error creating account. Please try again.";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEditAsync(short accountId, string accountName, string accountEmail, short accountRole, string? newPassword)
    {
        // Check if user is admin
        if (!SessionHelper.IsAdmin(HttpContext.Session))
        {
            return RedirectToPage("/Login");
        }

        try
        {
            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null)
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
            if (existingAccount != null && existingAccount.AccountId != accountId)
            {
                TempData["ErrorMessage"] = "An account with this email already exists.";
                return RedirectToPage();
            }

            account.AccountName = accountName;
            account.AccountEmail = accountEmail;
            account.AccountRole = accountRole;

            // Update password if provided
            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                account.AccountPassword = newPassword; // In real app, should hash this
            }

            var updatedAccount = await _accountService.UpdateAccountAsync(account);
            TempData["SuccessMessage"] = "Account updated successfully!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error updating account. Please try again.";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(short accountId)
    {
        // Check if user is admin
        if (!SessionHelper.IsAdmin(HttpContext.Session))
        {
            return RedirectToPage("/Login");
        }

        try
        {
            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null)
            {
                TempData["ErrorMessage"] = "Account not found.";
                return RedirectToPage();
            }

            // Prevent deleting admin account
            if (account.AccountEmail == "admin@FUNewsManagementSystem.org")
            {
                TempData["ErrorMessage"] = "Cannot delete the default admin account.";
                return RedirectToPage();
            }

            // Check if account has created news articles
            if (await _accountService.HasCreatedNewsAsync(accountId))
            {
                TempData["ErrorMessage"] = "Cannot delete account that has created news articles. Please reassign or delete the news articles first.";
                return RedirectToPage();
            }

            var success = await _accountService.DeleteAccountAsync(accountId);
            if (success)
            {
                TempData["SuccessMessage"] = "Account deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete account.";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error deleting account. Please try again.";
        }

        return RedirectToPage();
    }

    private async Task LoadDataAsync()
    {
        var allAccounts = await _accountService.GetAllAccountsAsync();

        // Apply search filter
        if (!string.IsNullOrEmpty(SearchTerm))
        {
            allAccounts = allAccounts.Where(a =>
                a.AccountName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                a.AccountEmail.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        // Apply role filter
        if (RoleFilter.HasValue)
        {
            allAccounts = allAccounts.Where(a => a.AccountRole == RoleFilter.Value);
        }

        Accounts = allAccounts.OrderBy(a => a.AccountRole).ThenBy(a => a.AccountName);
    }
}
