using FUNewsManagement.Models;

namespace FUNewsManagement.Repositories;

public interface ITagRepository : IGenericRepository<Tag>
{
    Task<IEnumerable<Tag>> SearchTagsAsync(string searchTerm);
    Task<Tag?> GetByNameAsync(string tagName);
}
