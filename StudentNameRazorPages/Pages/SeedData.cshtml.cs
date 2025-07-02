using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FUNewsManagement.Models;
using FUNewsManagement.Services;

namespace StudentNameRazorPages.Pages;

public class SeedDataModel : PageModel
{
    private readonly ISystemAccountService _accountService;
    private readonly ITagService _tagService;
    private readonly INewsArticleService _newsService;

    public SeedDataModel(
        ISystemAccountService accountService,
        ITagService tagService,
        INewsArticleService newsService)
    {
        _accountService = accountService;
        _tagService = tagService;
        _newsService = newsService;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            // Create sample accounts
            await CreateSampleAccountsAsync();

            // Create sample tags
            await CreateSampleTagsAsync();

            // Create sample news articles
            await CreateSampleNewsAsync();

            TempData["SuccessMessage"] = "Sample data created successfully! You can now login with the sample accounts.";
            return Page();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error creating sample data: {ex.Message}";
            return Page();
        }
    }

    private async Task CreateSampleAccountsAsync()
    {
        var accounts = new[]
        {
            new SystemAccount
            {
                AccountName = "John Staff",
                AccountEmail = "staff@funews.com",
                AccountPassword = "123456",
                AccountRole = 1 // Staff
            },
            new SystemAccount
            {
                AccountName = "Jane Staff",
                AccountEmail = "jane.staff@funews.com",
                AccountPassword = "123456",
                AccountRole = 1 // Staff
            },
            new SystemAccount
            {
                AccountName = "Dr. Smith",
                AccountEmail = "lecturer@funews.com",
                AccountPassword = "123456",
                AccountRole = 2 // Lecturer
            },
            new SystemAccount
            {
                AccountName = "Prof. Johnson",
                AccountEmail = "prof.johnson@funews.com",
                AccountPassword = "123456",
                AccountRole = 2 // Lecturer
            }
        };

        foreach (var account in accounts)
        {
            try
            {
                var existingAccount = await _accountService.GetAccountByEmailAsync(account.AccountEmail);
                if (existingAccount == null)
                {
                    await _accountService.CreateAccountAsync(account);
                }
            }
            catch
            {
                // Account might already exist, continue
            }
        }
    }

    private async Task CreateSampleTagsAsync()
    {
        var tags = new[]
        {
            new Tag { TagName = "University", Note = "University related news" },
            new Tag { TagName = "Student", Note = "Student activities and news" },
            new Tag { TagName = "Academic", Note = "Academic programs and courses" },
            new Tag { TagName = "Research", Note = "Research activities and findings" },
            new Tag { TagName = "Event", Note = "University events and activities" },
            new Tag { TagName = "Announcement", Note = "Official announcements" },
            new Tag { TagName = "Technology", Note = "Technology and IT related" },
            new Tag { TagName = "Sports", Note = "Sports and athletics" },
            new Tag { TagName = "Culture", Note = "Cultural activities and events" },
            new Tag { TagName = "International", Note = "International programs and partnerships" }
        };

        foreach (var tag in tags)
        {
            try
            {
                var existingTag = await _tagService.GetTagByNameAsync(tag.TagName);
                if (existingTag == null)
                {
                    await _tagService.CreateTagAsync(tag);
                }
            }
            catch
            {
                // Tag might already exist, continue
            }
        }
    }

    private async Task CreateSampleNewsAsync()
    {
        // Get staff account for creating news
        var staffAccount = await _accountService.GetAccountByEmailAsync("staff@funews.com");
        if (staffAccount == null) return;

        var sampleNews = new[]
        {
            new NewsArticle
            {
                NewsTitle = "Welcome to New Academic Year 2024",
                Headline = "University welcomes students for the new academic year with exciting programs",
                NewsContent = "The university is excited to welcome all students for the new academic year 2024. We have prepared various programs and activities to ensure a successful and enriching educational experience. New facilities have been added, and our faculty is ready to support students in their academic journey.",
                CategoryId = 1, // General News
                NewsStatus = true,
                CreatedById = staffAccount.AccountId,
                CreatedDate = DateTime.Now.AddDays(-5),
                NewsSource = "University Administration"
            },
            new NewsArticle
            {
                NewsTitle = "New Research Center Opens",
                Headline = "State-of-the-art research facility now available for students and faculty",
                NewsContent = "The university has officially opened its new research center, equipped with the latest technology and equipment. This facility will support various research projects across different disciplines and provide students with hands-on research experience.",
                CategoryId = 2, // Academic
                NewsStatus = true,
                CreatedById = staffAccount.AccountId,
                CreatedDate = DateTime.Now.AddDays(-3),
                NewsSource = "Research Department"
            },
            new NewsArticle
            {
                NewsTitle = "Annual Sports Festival 2024",
                Headline = "Join us for the biggest sports event of the year",
                NewsContent = "The annual sports festival will take place next month with various competitions and activities. Students from all departments are encouraged to participate. Registration is now open for individual and team events.",
                CategoryId = 3, // Events
                NewsStatus = true,
                CreatedById = staffAccount.AccountId,
                CreatedDate = DateTime.Now.AddDays(-1),
                NewsSource = "Sports Department"
            }
        };

        foreach (var news in sampleNews)
        {
            try
            {
                await _newsService.CreateNewsAsync(news, new List<int> { 1, 2 }); // Add some tags
            }
            catch
            {
                // News might already exist, continue
            }
        }
    }
}
