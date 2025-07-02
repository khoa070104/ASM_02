using FUNewsManagement.Models;
using FUNewsManagement.Repositories;

namespace FUNewsManagement.Services.Impl;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await _unitOfWork.Categories.GetAllAsync();
    }

    public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
    {
        return await _unitOfWork.Categories.GetActiveCategoriesAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(short id)
    {
        return await _unitOfWork.Categories.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Category>> GetParentCategoriesAsync()
    {
        return await _unitOfWork.Categories.GetParentCategoriesAsync();
    }

    public async Task<IEnumerable<Category>> GetSubCategoriesAsync(short parentId)
    {
        return await _unitOfWork.Categories.GetSubCategoriesAsync(parentId);
    }

    public async Task<Category> CreateCategoryAsync(Category category)
    {
        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateCategoryAsync(Category category)
    {
        _unitOfWork.Categories.Update(category);
        await _unitOfWork.SaveChangesAsync();
        return category;
    }

    public async Task<bool> DeleteCategoryAsync(short id)
    {
        if (!await CanDeleteCategoryAsync(id))
        {
            throw new InvalidOperationException("Cannot delete category that has news articles.");
        }

        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null)
            return false;

        _unitOfWork.Categories.Remove(category);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Category>> SearchCategoriesAsync(string searchTerm)
    {
        return await _unitOfWork.Categories.SearchCategoriesAsync(searchTerm);
    }

    public async Task<bool> CanDeleteCategoryAsync(short id)
    {
        return !await _unitOfWork.Categories.HasNewsArticlesAsync(id);
    }
}
