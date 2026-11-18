using Microsoft.EntityFrameworkCore;
using POS.API.Dtos.Products;
using POS.Data;
using POS.Models;

namespace POS.API.Services;

public class ProductService
{
    private readonly AppDbContext _db;

    public ProductService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<ProductResponseDto>>> GetProductsAsync()
    {
        var products = await ProductResponses()
            .OrderBy(product => product.Name)
            .ToListAsync();

        return Result<List<ProductResponseDto>>.Success(
            products,
            "Products retrieved successfully.");
    }

    public async Task<Result<ProductResponseDto>> GetProductAsync(int id)
    {
        var product = await ProductResponses()
            .FirstOrDefaultAsync(product => product.Id == id);

        return product is null
            ? Result<ProductResponseDto>.ValidationError("Product not found.")
            : Result<ProductResponseDto>.Success(
                product,
                "Product retrieved successfully.");
    }

    public async Task<Result<ProductResponseDto>> CreateProductAsync(CreateProductDto request)
    {
        var entity = new Product
        {
            ProductName = request.Name,
            ProductCode = request.Code,
            CatId = request.CategoryId,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            ReorderLevel = request.ReorderLevel
        };

        _db.Products.Add(entity);
        await _db.SaveChangesAsync();

        var response = await ProductResponses()
            .FirstOrDefaultAsync(product => product.Id == entity.ProductId);

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
        UpdateProductDto request)
    {
        var entity = await _db.Products
            .FirstOrDefaultAsync(product => product.ProductId == id);

        if (entity is null)
        {
            return Result<ProductResponseDto>.ValidationError("Product not found.");
        }

        entity.ProductName = request.Name;
        entity.ProductCode = request.Code;
        entity.CatId = request.CategoryId;
        entity.Price = request.Price;
        entity.StockQuantity = request.StockQuantity;
        entity.ReorderLevel = request.ReorderLevel;

        await _db.SaveChangesAsync();

        var response = await ProductResponses()
            .FirstOrDefaultAsync(product => product.Id == id);

        if (response is null)
        {
            return Result<ProductResponseDto>.SystemError(
                "The product was updated but could not be retrieved.");
        }

        return Result<ProductResponseDto>.Success(
            response,
            "Product updated successfully.");
    }

    public async Task<Result<ProductResponseDto>> DeleteProductAsync(int id)
    {
        var product = await ProductResponses()
            .FirstOrDefaultAsync(product => product.Id == id);

        await _db.Products
            .Where(product => product.ProductId == id)
            .ExecuteDeleteAsync();

        return Result<ProductResponseDto>.Success(
            "Product deleted successfully.",
            product);
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
}
