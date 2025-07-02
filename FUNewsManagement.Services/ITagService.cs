using FUNewsManagement.Models;

namespace FUNewsManagement.Services;

public interface ITagService
{
    Task<IEnumerable<Tag>> GetAllTagsAsync();
    Task<Tag?> GetTagByIdAsync(int id);
    Task<Tag?> GetTagByNameAsync(string name);
    Task<Tag> CreateTagAsync(Tag tag);
    Task<Tag> UpdateTagAsync(Tag tag);
    Task<bool> DeleteTagAsync(int id);
    Task<IEnumerable<Tag>> SearchTagsAsync(string searchTerm);
}
