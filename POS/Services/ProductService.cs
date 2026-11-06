using Microsoft.EntityFrameworkCore;
using POS.API.ViewModels;
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

    public async Task<List<ProductViewModel>> GetProductsAsync()
    {
        return await _db.Products.AsNoTracking()
            .Select(x => new ProductViewModel
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                ProductCode = x.ProductCode,
                CatId = x.CatId,
                Price = x.Price
            })
            .ToListAsync();
    }

    public async Task<ProductViewModel?> GetProductAsync(int id)
    {
        return await _db.Products
            .AsNoTracking()
            .Where(x => x.ProductId == id)
            .Select(x => new ProductViewModel
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                ProductCode = x.ProductCode,
                CatId = x.CatId,
                Price = x.Price
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ProductViewModel> CreateProductAsync(ProductViewModel product)
    {
        var entity = new Product
        {
            ProductName = product.ProductName,
            ProductCode = product.ProductCode,
            CatId = product.CatId,
            Price = product.Price
        };

        await _db.Products.AddAsync(entity);
        await _db.SaveChangesAsync();

        product.ProductId = entity.ProductId;
        return product;
    }

    public async Task<ProductViewModel?> UpdateProductAsync(int id, ProductViewModel product)
    {
        var entity = await _db.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ProductId == id);
        if (entity is null)
        {
            return null;
        }

        entity.ProductName = product.ProductName;
        entity.ProductCode = product.ProductCode;
        entity.CatId = product.CatId;
        entity.Price = product.Price;
        _db.Entry(entity).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return product;
    }

    public async Task DeleteProductAsync(int id)
    {
        await _db.Products
            .Where(x => x.ProductId == id)
            .ExecuteDeleteAsync();
    }
}
