using Microsoft.EntityFrameworkCore;
using POS.API.Dtos.Categories;
using POS.Data;
using POS.Models;

namespace POS.API.Services;

public class CategoryService
{
    private const int MaximumNameLength = 50;
    private readonly AppDbContext _db;

    public CategoryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<CategoryResponseDto>>> GetCategoriesAsync(
        CancellationToken cancellationToken = default)
    {
        var categories = await _db.Categories
            .AsNoTracking()
            .OrderBy(category => category.CatName)
            .Select(category => new CategoryResponseDto
            {
                Id = category.CatId,
                Name = category.CatName
            })
            .ToListAsync(cancellationToken);

        return Result<List<CategoryResponseDto>>.Success(
            categories,
            "Categories retrieved successfully.");
    }

    public async Task<Result<CategoryResponseDto>> GetCategoryAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result<CategoryResponseDto>.ValidationError(
                "Category id must be greater than zero.");
        }

        var category = await _db.Categories
            .AsNoTracking()
            .Where(entity => entity.CatId == id)
            .Select(entity => new CategoryResponseDto
            {
                Id = entity.CatId,
                Name = entity.CatName
            })
            .FirstOrDefaultAsync(cancellationToken);

        return category is null
            ? Result<CategoryResponseDto>.NotFoundError($"Category {id} does not exist.")
            : Result<CategoryResponseDto>.Success(
                category,
                "Category retrieved successfully.");
    }

    public async Task<Result<CategoryResponseDto>> CreateCategoryAsync(
        CreateCategoryDto category,
        CancellationToken cancellationToken = default)
    {
        var name = NormalizeName(category.Name);
        var validationMessage = ValidateName(name);

        if (validationMessage is not null)
        {
            return Result<CategoryResponseDto>.ValidationError(validationMessage);
        }

        if (await CategoryNameExistsAsync(name, excludedCategoryId: null, cancellationToken))
        {
            return Result<CategoryResponseDto>.ConflictError(
                $"A category named '{name}' already exists.");
        }

        var entity = new Category
        {
            CatName = name
        };

        _db.Categories.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        var response = ToResponseDto(entity);

        return Result<CategoryResponseDto>.Success(
            response,
            "Category created successfully.");
    }

    public async Task<Result<CategoryResponseDto>> UpdateCategoryAsync(
        int id,
        UpdateCategoryDto category,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result<CategoryResponseDto>.ValidationError(
                "Category id must be greater than zero.");
        }

        var name = NormalizeName(category.Name);
        var validationMessage = ValidateName(name);

        if (validationMessage is not null)
        {
            return Result<CategoryResponseDto>.ValidationError(validationMessage);
        }

        var entity = await _db.Categories
            .FirstOrDefaultAsync(existing => existing.CatId == id, cancellationToken);

        if (entity is null)
        {
            return Result<CategoryResponseDto>.NotFoundError(
                $"Category {id} does not exist.");
        }

        if (await CategoryNameExistsAsync(name, id, cancellationToken))
        {
            return Result<CategoryResponseDto>.ConflictError(
                $"A category named '{name}' already exists.");
        }

        entity.CatName = name;
        await _db.SaveChangesAsync(cancellationToken);

        return Result<CategoryResponseDto>.Success(
            ToResponseDto(entity),
            "Category updated successfully.");
    }

    public async Task<Result<CategoryResponseDto>> DeleteCategoryAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result<CategoryResponseDto>.ValidationError(
                "Category id must be greater than zero.");
        }

        var category = await _db.Categories
            .AsNoTracking()
            .Where(entity => entity.CatId == id)
            .Select(entity => new CategoryResponseDto
            {
                Id = entity.CatId,
                Name = entity.CatName
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (category is null)
        {
            return Result<CategoryResponseDto>.NotFoundError(
                $"Category {id} does not exist.");
        }

        var isInUse = await _db.Products
            .AsNoTracking()
            .AnyAsync(product => product.CatId == id, cancellationToken);

        if (isInUse)
        {
            return Result<CategoryResponseDto>.ConflictError(
                "The category cannot be deleted because products use it.");
        }

        await _db.Categories
            .Where(entity => entity.CatId == id)
            .ExecuteDeleteAsync(cancellationToken);

        return Result<CategoryResponseDto>.Success(
            category,
            "Category deleted successfully.");
    }

    private async Task<bool> CategoryNameExistsAsync(
        string name,
        int? excludedCategoryId,
        CancellationToken cancellationToken)
    {
        var normalizedName = name.ToUpperInvariant();

        return await _db.Categories
            .AsNoTracking()
            .AnyAsync(
                category =>
                    category.CatName.Trim().ToUpper() == normalizedName &&
                    (!excludedCategoryId.HasValue || category.CatId != excludedCategoryId.Value),
                cancellationToken);
    }

    private static string NormalizeName(string? name) =>
        name?.Trim() ?? string.Empty;

    private static string? ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return "Category name is required.";
        }

        return name.Length > MaximumNameLength
            ? $"Category name cannot exceed {MaximumNameLength} characters."
            : null;
    }

    private static CategoryResponseDto ToResponseDto(Category category) =>
        new()
        {
            Id = category.CatId,
            Name = category.CatName
        };

}
