using FUNewsManagement.Models;
using FUNewsManagement.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagement.Services.Impl;

public class NewsArticleService : INewsArticleService
{
    private readonly IUnitOfWork _unitOfWork;

    public NewsArticleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<NewsArticle>> GetAllNewsAsync()
    {
        return await _unitOfWork.NewsArticles.GetAllAsync();
    }

    public async Task<IEnumerable<NewsArticle>> GetActiveNewsAsync()
    {
        return await _unitOfWork.NewsArticles.GetActiveNewsAsync();
    }

    public async Task<NewsArticle?> GetNewsByIdAsync(string id)
    {
        return await _unitOfWork.NewsArticles.GetNewsWithDetailsAsync(id);
    }

    public async Task<IEnumerable<NewsArticle>> GetNewsByCategoryAsync(short categoryId)
    {
        return await _unitOfWork.NewsArticles.GetNewsByCategoryAsync(categoryId);
    }

    public async Task<IEnumerable<NewsArticle>> GetNewsByAuthorAsync(short authorId)
    {
        return await _unitOfWork.NewsArticles.GetNewsByAuthorAsync(authorId);
    }

    public async Task<NewsArticle> CreateNewsAsync(NewsArticle news, List<int> tagIds)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            news.NewsArticleId = await GenerateNewsIdAsync();
            news.CreatedDate = DateTime.Now;

            await _unitOfWork.NewsArticles.AddAsync(news);
            await _unitOfWork.SaveChangesAsync();

            // Add tags will be handled in repository layer
            if (tagIds.Any())
            {
                // This will be implemented in a separate method
                await AddNewsTagsAsync(news.NewsArticleId, tagIds);
            }

            await _unitOfWork.CommitTransactionAsync();
            return news;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<NewsArticle> UpdateNewsAsync(NewsArticle news, List<int> tagIds)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            news.ModifiedDate = DateTime.Now;
            _unitOfWork.NewsArticles.Update(news);

            // Remove existing tags and add new ones
            await UpdateNewsTagsAsync(news.NewsArticleId, tagIds);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
            return news;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<bool> DeleteNewsAsync(string id)
    {
        var news = await _unitOfWork.NewsArticles.GetByIdAsync(id);
        if (news == null)
            return false;

        _unitOfWork.NewsArticles.Remove(news);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<NewsArticle>> SearchNewsAsync(string searchTerm)
    {
        return await _unitOfWork.NewsArticles.SearchNewsAsync(searchTerm);
    }

    public async Task<IEnumerable<NewsArticle>> GetNewsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _unitOfWork.NewsArticles.GetNewsByDateRangeAsync(startDate, endDate);
    }

    public async Task<string> GenerateNewsIdAsync()
    {
        string prefix = "NEWS";
        string datePart = DateTime.Now.ToString("yyyyMMdd");

        // Find the highest sequence number for today
        var todayNews = await _unitOfWork.NewsArticles.FindAsync(n =>
            n.NewsArticleId.StartsWith(prefix + datePart));

        int sequence = 1;
        if (todayNews.Any())
        {
            var maxSequence = todayNews
                .Select(n => n.NewsArticleId.Substring(12)) // Remove "NEWSyyyyMMdd"
                .Where(s => int.TryParse(s, out _))
                .Select(int.Parse)
                .DefaultIfEmpty(0)
                .Max();

            sequence = maxSequence + 1;
        }

        return $"{prefix}{datePart}{sequence:D4}";
    }

    private async Task AddNewsTagsAsync(string newsArticleId, List<int> tagIds)
    {
        // This would need to be implemented with a proper NewsTag repository
        // For now, we'll handle it in the main methods
    }

    private async Task UpdateNewsTagsAsync(string newsArticleId, List<int> tagIds)
    {
        // This would need to be implemented with a proper NewsTag repository
        // For now, we'll handle it in the main methods
    }
}
