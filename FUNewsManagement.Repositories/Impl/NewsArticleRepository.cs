using Microsoft.EntityFrameworkCore;
using FUNewsManagement.Models;

namespace FUNewsManagement.Repositories.Impl;

public class NewsArticleRepository : GenericRepository<NewsArticle>, INewsArticleRepository
{
    public NewsArticleRepository(FUNewsManagementDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<NewsArticle>> GetActiveNewsAsync()
    {
        return await _dbSet
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.UpdatedBy)
            .Where(n => n.NewsStatus)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<NewsArticle>> GetNewsByCategoryAsync(short categoryId)
    {
        return await _dbSet
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.UpdatedBy)
            .Where(n => n.CategoryId == categoryId && n.NewsStatus)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<NewsArticle>> GetNewsByAuthorAsync(short authorId)
    {
        return await _dbSet
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.UpdatedBy)
            .Where(n => n.CreatedById == authorId)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<NewsArticle>> GetNewsWithTagsAsync()
    {
        return await _dbSet
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.UpdatedBy)
            .Include(n => n.NewsTags)
                .ThenInclude(nt => nt.Tag)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<NewsArticle>> SearchNewsAsync(string searchTerm)
    {
        return await _dbSet
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.UpdatedBy)
            .Where(n => n.NewsTitle.Contains(searchTerm) || 
                       n.NewsContent.Contains(searchTerm) ||
                       (n.Headline != null && n.Headline.Contains(searchTerm)))
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<NewsArticle>> GetNewsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.UpdatedBy)
            .Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }

    public async Task<NewsArticle?> GetNewsWithDetailsAsync(string newsId)
    {
        return await _dbSet
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.UpdatedBy)
            .Include(n => n.NewsTags)
                .ThenInclude(nt => nt.Tag)
            .FirstOrDefaultAsync(n => n.NewsArticleId == newsId);
    }

    public override async Task<IEnumerable<NewsArticle>> GetAllAsync()
    {
        return await _dbSet
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.UpdatedBy)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }

    public override async Task<NewsArticle?> GetByIdAsync(object id)
    {
        return await GetNewsWithDetailsAsync((string)id);
    }
}
