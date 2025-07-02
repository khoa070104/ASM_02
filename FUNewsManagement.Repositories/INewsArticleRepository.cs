using FUNewsManagement.Models;

namespace FUNewsManagement.Repositories;

public interface INewsArticleRepository : IGenericRepository<NewsArticle>
{
    Task<IEnumerable<NewsArticle>> GetActiveNewsAsync();
    Task<IEnumerable<NewsArticle>> GetNewsByCategoryAsync(short categoryId);
    Task<IEnumerable<NewsArticle>> GetNewsByAuthorAsync(short authorId);
    Task<IEnumerable<NewsArticle>> GetNewsWithTagsAsync();
    Task<IEnumerable<NewsArticle>> SearchNewsAsync(string searchTerm);
    Task<IEnumerable<NewsArticle>> GetNewsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<NewsArticle?> GetNewsWithDetailsAsync(string newsId);
}
