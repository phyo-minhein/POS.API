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

    public async Task<Result<List<CategoryViewModel>>> GetCategoriesAsync()
    {
        Result<List<CategoryViewModel>> model;
        var list = await _db.Categories.AsNoTracking()
            .Select(x => new CategoryViewModel
            {
                CatId = x.CatId,
                CatName = x.CatName
            })
            .ToListAsync();
        model = Result<List<CategoryViewModel>>.Success("Categories retrieved successfully", list);
        return model;
    }

    public async Task<Result<CategoryViewModel>> GetCategoryAsync(int id)
    {
        Result<CategoryViewModel> model;
        var entity = await _db.Categories.FirstOrDefaultAsync(x => x.CatId == id);
        if(entity is null)
        {
            model = Result<CategoryViewModel>.ValidationError("Category not found");
            goto Response;
        }
        var cat = new CategoryViewModel
        {
            CatId = entity.CatId,
            CatName = entity.CatName
        };
        model = Result<CategoryViewModel>.Success("Category retrieved successfully", cat);

        Response:
        return model;
    }

    public async Task<Result<CategoryViewModel>> CreateCategoryAsync(CategoryViewModel category)
    {
        Result<CategoryViewModel> model;
        var entity = new Category
        {
            CatName = category.CatName
        };

        await _db.Categories.AddAsync(entity);
        await _db.SaveChangesAsync();

        category.CatId = entity.CatId;
        model = Result<CategoryViewModel>.Success("Category created successfully", category);
        return model;
    }

    public async Task<Result<CategoryViewModel>> UpdateCategoryAsync(int id, CategoryViewModel category)
    {
        Result<CategoryViewModel> model;
        var entity = await _db.Categories.FirstOrDefaultAsync(x => x.CatId == id);
        if (entity is null)
        {
            model = Result<CategoryViewModel>.ValidationError("Category not found");
            goto Response;
        }
        entity.CatName = category.CatName;
        await _db.SaveChangesAsync();
        model = Result<CategoryViewModel>.Success("Category updated successfully", category);

    Response:
        return model;
    }

    public async Task<Result<CategoryViewModel>> DeleteCategoryAsync(int id)
    {
        Result<CategoryViewModel> model;
        await _db.Categories
            .Where(x => x.CatId == id)
            .ExecuteDeleteAsync();
        model = Result<CategoryViewModel>.Success();
        return model;
    }
}
