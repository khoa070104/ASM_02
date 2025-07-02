using FUNewsManagement.Models;

namespace FUNewsManagement.Services;

public interface INewsArticleService
{
    Task<IEnumerable<NewsArticle>> GetAllNewsAsync();
    Task<IEnumerable<NewsArticle>> GetActiveNewsAsync();
    Task<NewsArticle?> GetNewsByIdAsync(string id);
    Task<IEnumerable<NewsArticle>> GetNewsByCategoryAsync(short categoryId);
    Task<IEnumerable<NewsArticle>> GetNewsByAuthorAsync(short authorId);
    Task<NewsArticle> CreateNewsAsync(NewsArticle news, List<int> tagIds);
    Task<NewsArticle> UpdateNewsAsync(NewsArticle news, List<int> tagIds);
    Task<bool> DeleteNewsAsync(string id);
    Task<IEnumerable<NewsArticle>> SearchNewsAsync(string searchTerm);
    Task<IEnumerable<NewsArticle>> GetNewsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<string> GenerateNewsIdAsync();
}
