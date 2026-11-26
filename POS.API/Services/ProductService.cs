using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using POS.API.Dtos.Common;
using POS.API.Dtos.Products;
using POS.Data;
using POS.Models;

namespace POS.API.Services;

public class ProductService
{
    private const int MaximumNameLength = 100;
    private const int MaximumCodeLength = 10;
    private const decimal MaximumPrice = 99_999_999.99m;
    private readonly AppDbContext _db;

    public ProductService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<PagedResult<ProductResponseDto>>> GetProductsPagingAsync(
        ProductParamDto request,
        CancellationToken cancellationToken = default)
    {
        var search = Normalize(request.Search);
        var sortBy = Normalize(request.SortBy).ToLowerInvariant();

        if (string.IsNullOrEmpty(sortBy))
        {
            sortBy = "name";
        }

        var validationMessage = ValidateProductParam(request, search, sortBy);

        if (validationMessage is not null)
        {
            return Result<PagedResult<ProductResponseDto>>.ValidationError(validationMessage);
        }

        var query = ProductResponses();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(product =>
                product.Name.Contains(search) ||
                product.Code.Contains(search));
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(product => product.CategoryId == request.CategoryId.Value);
        }

        if (request.LowStockOnly)
        {
            query = query.Where(product => product.StockQuantity <= product.ReorderLevel);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var orderedQuery = OrderProducts(query, sortBy, request.SortDescending);
        var skip = ((long)request.Page - 1) * request.PageSize;
        var products = skip >= totalCount
            ? []
            : await orderedQuery
                .Skip((int)skip)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

        var page = new PagedResult<ProductResponseDto>
        {
            Items = products,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };

        return Result<PagedResult<ProductResponseDto>>.Success(
            page,
            "Products retrieved successfully.");
    }

    public async Task<Result<ProductResponseDto>> GetProductAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result<ProductResponseDto>.ValidationError(
                "Product id must be greater than zero.");
        }

        var product = await ProductResponses()
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);

        return product is null
            ? Result<ProductResponseDto>.NotFoundError($"Product {id} does not exist.")
            : Result<ProductResponseDto>.Success(
                product,
                "Product retrieved successfully.");
    }

    public async Task<Result<ProductResponseDto>> CreateProductAsync(
        CreateProductRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var name = Normalize(request.Name);
        var code = Normalize(request.Code);
        var validationMessage = ValidateProduct(
            name,
            code,
            request.CategoryId,
            request.Price,
            request.StockQuantity,
            request.ReorderLevel);

        if (validationMessage is not null)
        {
            return Result<ProductResponseDto>.ValidationError(validationMessage);
        }

        if (!await CategoryExistsAsync(request.CategoryId, cancellationToken))
        {
            return Result<ProductResponseDto>.ValidationError(
                $"Category {request.CategoryId} does not exist.");
        }

        if (await ProductCodeExistsAsync(code, excludedProductId: null, cancellationToken))
        {
            return Result<ProductResponseDto>.ConflictError(
                $"A product with code '{code}' already exists.");
        }

        var entity = new Product
        {
            ProductName = name,
            ProductCode = code,
            CatId = request.CategoryId,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            ReorderLevel = request.ReorderLevel
        };

        _db.Products.Add(entity);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
        {
            return Result<ProductResponseDto>.ConflictError(
                $"A product with code '{code}' already exists.");
        }
        catch (DbUpdateException exception) when (IsForeignKeyViolation(exception))
        {
            return Result<ProductResponseDto>.ValidationError(
                "The selected category does not exist.");
        }

        var response = await ProductResponses()
            .FirstOrDefaultAsync(
                product => product.Id == entity.ProductId,
                cancellationToken);

        if (response is null)
        {
            return Result<ProductResponseDto>.SystemError(
                "The product was created but could not be retrieved.");
        }

        return Result<ProductResponseDto>.Success(
            response,
            "Product created successfully.");
    }

    public async Task<Result<ProductResponseDto>> UpdateProductAsync(
        int id,
        UpdateProductRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result<ProductResponseDto>.ValidationError(
                "Product id must be greater than zero.");
        }

        var name = Normalize(request.Name);
        var code = Normalize(request.Code);
        var validationMessage = ValidateProduct(
            name,
            code,
            request.CategoryId,
            request.Price,
            request.StockQuantity,
            request.ReorderLevel);

        if (validationMessage is not null)
        {
            return Result<ProductResponseDto>.ValidationError(validationMessage);
        }

        var entity = await _db.Products
            .FirstOrDefaultAsync(product => product.ProductId == id, cancellationToken);

        if (entity is null)
        {
            return Result<ProductResponseDto>.NotFoundError(
                $"Product {id} does not exist.");
        }

        if (!await CategoryExistsAsync(request.CategoryId, cancellationToken))
        {
            return Result<ProductResponseDto>.ValidationError(
                $"Category {request.CategoryId} does not exist.");
        }

        if (await ProductCodeExistsAsync(code, id, cancellationToken))
        {
            return Result<ProductResponseDto>.ConflictError(
                $"A product with code '{code}' already exists.");
        }

        entity.ProductName = name;
        entity.ProductCode = code;
        entity.CatId = request.CategoryId;
        entity.Price = request.Price;
        entity.StockQuantity = request.StockQuantity;
        entity.ReorderLevel = request.ReorderLevel;

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
        {
            return Result<ProductResponseDto>.ConflictError(
                $"A product with code '{code}' already exists.");
        }
        catch (DbUpdateException exception) when (IsForeignKeyViolation(exception))
        {
            return Result<ProductResponseDto>.ValidationError(
                "The selected category does not exist.");
        }

        var response = await ProductResponses()
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);

        if (response is null)
        {
            return Result<ProductResponseDto>.SystemError(
                "The product was updated but could not be retrieved.");
        }

        return Result<ProductResponseDto>.Success(
            response,
            "Product updated successfully.");
    }

    public async Task<Result<ProductResponseDto>> DeleteProductAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result<ProductResponseDto>.ValidationError(
                "Product id must be greater than zero.");
        }

        var product = await ProductResponses()
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);

        if (product is null)
        {
            return Result<ProductResponseDto>.NotFoundError(
                $"Product {id} does not exist.");
        }

        var isInUse = await _db.SaleDetails
            .AsNoTracking()
            .AnyAsync(detail => detail.ProductId == id, cancellationToken);

        if (isInUse)
        {
            return Result<ProductResponseDto>.ConflictError(
                "The product cannot be deleted because it has been used in a sale.");
        }

        try
        {
            await _db.Products
                .Where(entity => entity.ProductId == id)
                .ExecuteDeleteAsync(cancellationToken);
        }
        catch (SqlException exception) when (exception.Number == 547)
        {
            return Result<ProductResponseDto>.ConflictError(
                "The product cannot be deleted because it has been used in a sale.");
        }

        return Result<ProductResponseDto>.Success(
            product,
            "Product deleted successfully.");
    }

    private async Task<bool> CategoryExistsAsync(
        int? categoryId,
        CancellationToken cancellationToken)
    {
        if (!categoryId.HasValue)
        {
            return true;
        }

        return await _db.Categories
            .AsNoTracking()
            .AnyAsync(category => category.CatId == categoryId.Value, cancellationToken);
    }

    private async Task<bool> ProductCodeExistsAsync(
        string code,
        int? excludedProductId,
        CancellationToken cancellationToken)
    {
        var normalizedCode = code.ToUpperInvariant();

        return await _db.Products
            .AsNoTracking()
            .AnyAsync(
                product =>
                    product.ProductCode.Trim().ToUpper() == normalizedCode &&
                    (!excludedProductId.HasValue || product.ProductId != excludedProductId.Value),
                cancellationToken);
    }

    private IQueryable<ProductResponseDto> ProductResponses() =>
        _db.Products
            .AsNoTracking()
            .Select(product => new ProductResponseDto
            {
                Id = product.ProductId,
                Name = product.ProductName,
                Code = product.ProductCode,
                CategoryId = product.CatId,
                CategoryName = product.Cat == null ? null : product.Cat.CatName,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ReorderLevel = product.ReorderLevel,
                IsLowStock = product.StockQuantity <= product.ReorderLevel
            });

    private static IOrderedQueryable<ProductResponseDto> OrderProducts(
        IQueryable<ProductResponseDto> query,
        string sortBy,
        bool descending)
    {
        IOrderedQueryable<ProductResponseDto> orderedQuery = (sortBy, descending) switch
        {
            ("code", false) => query.OrderBy(product => product.Code),
            ("code", true) => query.OrderByDescending(product => product.Code),
            ("category", false) => query.OrderBy(product => product.CategoryName),
            ("category", true) => query.OrderByDescending(product => product.CategoryName),
            ("price", false) => query.OrderBy(product => product.Price),
            ("price", true) => query.OrderByDescending(product => product.Price),
            ("stockquantity", false) => query.OrderBy(product => product.StockQuantity),
            ("stockquantity", true) => query.OrderByDescending(product => product.StockQuantity),
            ("reorderlevel", false) => query.OrderBy(product => product.ReorderLevel),
            ("reorderlevel", true) => query.OrderByDescending(product => product.ReorderLevel),
            ("name", true) => query.OrderByDescending(product => product.Name),
            _ => query.OrderBy(product => product.Name)
        };

        return orderedQuery.ThenBy(product => product.Id);
    }

    private static string Normalize(string? value) =>
        value?.Trim() ?? string.Empty;

    private static string? ValidateProduct(
        string name,
        string code,
        int? categoryId,
        decimal price,
        int stockQuantity,
        int reorderLevel)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return "Product name is required.";
        }

        if (name.Length > MaximumNameLength)
        {
            return $"Product name cannot exceed {MaximumNameLength} characters.";
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            return "Product code is required.";
        }

        if (code.Length > MaximumCodeLength)
        {
            return $"Product code cannot exceed {MaximumCodeLength} characters.";
        }

        if (categoryId <= 0)
        {
            return "Category id must be greater than zero.";
        }

        if (price < 0 || price > MaximumPrice)
        {
            return $"Price must be between 0 and {MaximumPrice}.";
        }

        if (stockQuantity < 0)
        {
            return "Stock quantity cannot be negative.";
        }

        return reorderLevel < 0
            ? "Reorder level cannot be negative."
            : null;
    }

    private static string? ValidateProductParam(
        ProductParamDto request,
        string search,
        string sortBy)
    {
        if (search.Length > 100)
        {
            return "Search cannot exceed 100 characters.";
        }

        if (request.CategoryId <= 0)
        {
            return "Category id must be greater than zero.";
        }

        if (request.Page <= 0)
        {
            return "Page must be greater than zero.";
        }

        if (request.PageSize is < 1 or > 100)
        {
            return "Page size must be between 1 and 100.";
        }

        return sortBy is not ("name" or "code" or "category" or "price" or "stockquantity" or "reorderlevel")
            ? "Sort field must be name, code, category, price, stockQuantity, or reorderLevel."
            : null;
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException exception) =>
        exception.InnerException is SqlException { Number: 2601 or 2627 };

    private static bool IsForeignKeyViolation(DbUpdateException exception) =>
        exception.InnerException is SqlException { Number: 547 };
}
