using Microsoft.EntityFrameworkCore;
using POS.API.ViewModels;
using POS.Data;
using POS.Models;

namespace POS.API.Services;

public class CategoryService
{
    private readonly AppDbContext _db;

    public CategoryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<CategoryViewModel>> GetCategoriesAsync()
    {
        return await _db.Categories.AsNoTracking()
            .Select(x => new CategoryViewModel
            {
                CatId = x.CatId,
                CatName = x.CatName
            })
            .ToListAsync();
    }

    public async Task<CategoryViewModel?> GetCategoryAsync(int id)
    {
        return await _db.Categories
            .AsNoTracking()
            .Where(x => x.CatId == id)
            .Select(x => new CategoryViewModel
            {
                CatId = x.CatId,
                CatName = x.CatName
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CategoryViewModel> CreateCategoryAsync(CategoryViewModel category)
    {
        var entity = new Category
        {
            CatName = category.CatName
        };

        await _db.Categories.AddAsync(entity);
        await _db.SaveChangesAsync();

        category.CatId = entity.CatId;
        return category;
    }

    public async Task<CategoryViewModel?> UpdateCategoryAsync(int id, CategoryViewModel category)
    {
        var entity = await _db.Categories.FirstOrDefaultAsync(x => x.CatId == id);
        if (entity is null)
        {
            return null;
        }

        entity.CatName = category.CatName;
        await _db.SaveChangesAsync();
        return category;
    }

    public async Task DeleteCategoryAsync(int id)
    {
        await _db.Categories
            .Where(x => x.CatId == id)
            .ExecuteDeleteAsync();
    }
}
