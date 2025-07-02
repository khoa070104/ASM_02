using Microsoft.EntityFrameworkCore;
using FUNewsManagement.Models;

namespace FUNewsManagement.Repositories.Impl;

public class TagRepository : GenericRepository<Tag>, ITagRepository
{
    public TagRepository(FUNewsManagementDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Tag>> SearchTagsAsync(string searchTerm)
    {
        return await _dbSet
            .Where(t => t.TagName.Contains(searchTerm) || 
                       (t.Note != null && t.Note.Contains(searchTerm)))
            .ToListAsync();
    }

    public async Task<Tag?> GetByNameAsync(string tagName)
    {
        return await _dbSet.FirstOrDefaultAsync(t => t.TagName == tagName);
    }
}
