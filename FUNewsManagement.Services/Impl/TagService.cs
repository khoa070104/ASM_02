using FUNewsManagement.Models;
using FUNewsManagement.Repositories;

namespace FUNewsManagement.Services.Impl;

public class TagService : ITagService
{
    private readonly IUnitOfWork _unitOfWork;

    public TagService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Tag>> GetAllTagsAsync()
    {
        return await _unitOfWork.Tags.GetAllAsync();
    }

    public async Task<Tag?> GetTagByIdAsync(int id)
    {
        return await _unitOfWork.Tags.GetByIdAsync(id);
    }

    public async Task<Tag?> GetTagByNameAsync(string name)
    {
        return await _unitOfWork.Tags.GetByNameAsync(name);
    }

    public async Task<Tag> CreateTagAsync(Tag tag)
    {
        var existingTag = await _unitOfWork.Tags.GetByNameAsync(tag.TagName);
        if (existingTag != null)
        {
            throw new InvalidOperationException("Tag name already exists.");
        }

        await _unitOfWork.Tags.AddAsync(tag);
        await _unitOfWork.SaveChangesAsync();
        return tag;
    }

    public async Task<Tag> UpdateTagAsync(Tag tag)
    {
        var existingTag = await _unitOfWork.Tags.GetByNameAsync(tag.TagName);
        if (existingTag != null && existingTag.TagId != tag.TagId)
        {
            throw new InvalidOperationException("Tag name already exists.");
        }

        _unitOfWork.Tags.Update(tag);
        await _unitOfWork.SaveChangesAsync();
        return tag;
    }

    public async Task<bool> DeleteTagAsync(int id)
    {
        var tag = await _unitOfWork.Tags.GetByIdAsync(id);
        if (tag == null)
            return false;

        _unitOfWork.Tags.Remove(tag);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Tag>> SearchTagsAsync(string searchTerm)
    {
        return await _unitOfWork.Tags.SearchTagsAsync(searchTerm);
    }
}
