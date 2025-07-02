using Microsoft.EntityFrameworkCore;
using FUNewsManagement.Models;

namespace FUNewsManagement.Repositories.Impl;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(FUNewsManagementDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
    {
        return await _dbSet.Where(c => c.IsActive).ToListAsync();
    }

    public async Task<IEnumerable<Category>> GetParentCategoriesAsync()
    {
        return await _dbSet.Where(c => c.ParentCategoryId == null && c.IsActive).ToListAsync();
    }

    public async Task<IEnumerable<Category>> GetSubCategoriesAsync(short parentId)
    {
        return await _dbSet.Where(c => c.ParentCategoryId == parentId && c.IsActive).ToListAsync();
    }

    public async Task<bool> HasNewsArticlesAsync(short categoryId)
    {
        return await _context.NewsArticles.AnyAsync(n => n.CategoryId == categoryId);
    }

    public async Task<IEnumerable<Category>> SearchCategoriesAsync(string searchTerm)
    {
        return await _dbSet
            .Where(c => c.CategoryName.Contains(searchTerm) || 
                       (c.CategoryDescription != null && c.CategoryDescription.Contains(searchTerm)))
            .ToListAsync();
    }

    public override async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _dbSet
            .Include(c => c.ParentCategory)
            .Include(c => c.SubCategories)
            .ToListAsync();
    }

    public override async Task<Category?> GetByIdAsync(object id)
    {
        return await _dbSet
            .Include(c => c.ParentCategory)
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.CategoryId == (short)id);
    }
}
