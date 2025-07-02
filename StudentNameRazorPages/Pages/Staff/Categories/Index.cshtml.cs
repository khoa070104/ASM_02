using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FUNewsManagement.Models;
using StudentNameRazorPages.Helpers;
using FUNewsManagement.Services;

namespace StudentNameRazorPages.Pages.Staff.Categories;

public class IndexModel : PageModel
{
    private readonly ICategoryService _categoryService;

    public IndexModel(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    public IEnumerable<Category> ParentCategories { get; set; } = new List<Category>();
    public string SearchTerm { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(string? search)
    {
        // Check if user is staff
        if (!SessionHelper.IsStaff(HttpContext.Session))
        {
            return RedirectToPage("/Login");
        }

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

    public async Task<IActionResult> OnPostCreateAsync(string categoryName, string? categoryDescription, short? parentCategoryId, bool isActive)
    {
        // Check if user is staff
        if (!SessionHelper.IsStaff(HttpContext.Session))
        {
            return RedirectToPage("/Login");
        }

        try
        {
            var category = new Category
            {
                CategoryName = categoryName,
                CategoryDescription = categoryDescription,
                ParentCategoryId = parentCategoryId,
                IsActive = isActive
            };

            await _categoryService.CreateCategoryAsync(category);
            TempData["SuccessMessage"] = "Category created successfully!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error creating category. Please try again.";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEditAsync(short categoryId, string categoryName, string? categoryDescription, short? parentCategoryId, bool isActive)
    {
        // Check if user is staff
        if (!SessionHelper.IsStaff(HttpContext.Session))
        {
            return RedirectToPage("/Login");
        }

        try
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Category not found.";
                return RedirectToPage();
            }

            category.CategoryName = categoryName;
            category.CategoryDescription = categoryDescription;
            category.ParentCategoryId = parentCategoryId;
            category.IsActive = isActive;

            await _categoryService.UpdateCategoryAsync(category);
            TempData["SuccessMessage"] = "Category updated successfully!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error updating category. Please try again.";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(short categoryId)
    {
        // Check if user is staff
        if (!SessionHelper.IsStaff(HttpContext.Session))
        {
            return RedirectToPage("/Login");
        }

        try
        {
            // Check if category can be deleted (no news articles)
            if (!await _categoryService.CanDeleteCategoryAsync(categoryId))
            {
                TempData["ErrorMessage"] = "Cannot delete category that has news articles. Please remove all news articles first.";
                return RedirectToPage();
            }

            var success = await _categoryService.DeleteCategoryAsync(categoryId);
            if (success)
            {
                TempData["SuccessMessage"] = "Category deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Category not found.";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error deleting category. Please try again.";
        }

        return RedirectToPage();
    }

    private async Task LoadDataAsync()
    {
        var allCategories = await _categoryService.GetAllCategoriesAsync();

        // Apply search filter
        if (!string.IsNullOrEmpty(SearchTerm))
        {
            allCategories = allCategories.Where(c => 
                c.CategoryName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (c.CategoryDescription != null && c.CategoryDescription.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)));
        }

        Categories = allCategories.OrderBy(c => c.CategoryName);
        
        // Load parent categories (for dropdown)
        ParentCategories = (await _categoryService.GetParentCategoriesAsync()).OrderBy(c => c.CategoryName);
    }
}
