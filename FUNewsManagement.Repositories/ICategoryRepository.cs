using FUNewsManagement.Models;

namespace FUNewsManagement.Repositories;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<IEnumerable<Category>> GetActiveCategoriesAsync();
    Task<IEnumerable<Category>> GetParentCategoriesAsync();
    Task<IEnumerable<Category>> GetSubCategoriesAsync(short parentId);
    Task<bool> HasNewsArticlesAsync(short categoryId);
    Task<IEnumerable<Category>> SearchCategoriesAsync(string searchTerm);
}
