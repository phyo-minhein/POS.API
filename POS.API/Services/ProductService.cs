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

    public async Task<Result<List<ProductViewModel>>> GetProductsAsync()
    {
        Result<List<ProductViewModel>> model;
        var list = await _db.Products.AsNoTracking()
            .Select(x => new ProductViewModel
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                ProductCode = x.ProductCode,
                CatId = x.CatId,
                Price = x.Price
            })
            .ToListAsync();
        model = Result<List<ProductViewModel>>.Success("Products retrieved successfully", list);
        return model;
    }

    public async Task<Result<ProductViewModel>> GetProductAsync(int id)
    {
        Result<ProductViewModel> model;
        var entity = await _db.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ProductId == id);
        if (entity is null)
        {
            model = Result<ProductViewModel>.ValidationError("Product not found");
            goto Response;
        }
        var product = new ProductViewModel
        {
            ProductId = entity.ProductId,
            ProductName = entity.ProductName,
            ProductCode = entity.ProductCode,
            CatId = entity.CatId,
            Price = entity.Price
        };
        model = Result<ProductViewModel>.Success("Product retrieved successfully", product);

        Response:
        return model;
    }

    public async Task<Result<ProductViewModel>> CreateProductAsync(ProductViewModel product)
    {
        Result<ProductViewModel> model;
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
        model = Result<ProductViewModel>.Success("Product created successfully", product);
        return model;
    }

    public async Task<Result<ProductViewModel>> UpdateProductAsync(int id, ProductViewModel product)
    {
        Result<ProductViewModel> model;
        var entity = await _db.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ProductId == id);
        if (entity is null)
        {
            model = Result<ProductViewModel>.ValidationError("Product not found");
            goto Response;
        }

        entity.ProductName = product.ProductName;
        entity.ProductCode = product.ProductCode;
        entity.CatId = product.CatId;
        entity.Price = product.Price;
        _db.Entry(entity).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        model = Result<ProductViewModel>.Success("Product updated successfully", product);

    Response:
        return model;
    }

    public async Task<Result<ProductViewModel>> DeleteProductAsync(int id)
    {
        Result<ProductViewModel> model;
        await _db.Products
            .Where(x => x.ProductId == id)
            .ExecuteDeleteAsync();
        model = Result<ProductViewModel>.Success();
        return model;
    }
}
