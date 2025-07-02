using FUNewsManagement.Models;

namespace FUNewsManagement.Services;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<IEnumerable<Category>> GetActiveCategoriesAsync();
    Task<Category?> GetCategoryByIdAsync(short id);
    Task<IEnumerable<Category>> GetParentCategoriesAsync();
    Task<IEnumerable<Category>> GetSubCategoriesAsync(short parentId);
    Task<Category> CreateCategoryAsync(Category category);
    Task<Category> UpdateCategoryAsync(Category category);
    Task<bool> DeleteCategoryAsync(short id);
    Task<IEnumerable<Category>> SearchCategoriesAsync(string searchTerm);
    Task<bool> CanDeleteCategoryAsync(short id);
}
