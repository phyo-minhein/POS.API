using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS.API.ViewModels;
using POS.Data;
using POS.Models;

namespace POS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _db;

        public ProductController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsAsync()
        {
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
            return Ok(list);
        }

        [HttpGet("{id}", Name = "GetProductById")]
        public async Task<IActionResult> GetProductAsync(int id)
        {
            var product = await _db.Products
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

            if (product is null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductViewModel product)
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
            return Ok(product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductViewModel product)
        {
            var entity = await _db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == id);
            if (entity is null)
            {
                return NotFound();
            }
            entity.ProductName = product.ProductName;
            entity.ProductCode = product.ProductCode;
            entity.CatId = product.CatId;
            entity.Price = product.Price;
            _db.Entry(entity).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _db.Products.Where(x => x.ProductId == id).ExecuteDeleteAsync();
            return NoContent();
        }
    }
}
