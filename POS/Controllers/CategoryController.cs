using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS.API.ViewModels;
using POS.Data;
using POS.Models;

namespace POS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _db;

        public CategoryController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoriesAsync()
        {
            var list = await _db.Categories.AsNoTracking()
                                .Select(x => new CategoryViewModel
                                {
                                    CatId = x.CatId,
                                    CatName = x.CatName
                                })
                                .ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id}", Name = "GetCategoryById")]
        public async Task<IActionResult> GetCategoryAsync(int id)
        {
            var cat = await _db.Categories
                .AsNoTracking()
                .Where(x => x.CatId == id)
                .Select(x => new CategoryViewModel
                {
                    CatId = x.CatId,
                    CatName = x.CatName
                })
                .FirstOrDefaultAsync();

            if (cat is null)
            {
                return NotFound();
            }

            return Ok(cat);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryViewModel category)
        {
            var cat = new Category
            {
                CatName = category.CatName
            };
            await _db.Categories.AddAsync(cat);
            await _db.SaveChangesAsync();
            category.CatId = cat.CatId;
            return Ok(category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryViewModel category)
        {
            var cat = await _db.Categories.FirstOrDefaultAsync(x => x.CatId == id);
            if(cat is null)
            {
                return NotFound();
            }
            cat.CatName = category.CatName;
            await _db.SaveChangesAsync();
            return Ok(category);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _db.Categories.Where(x => x.CatId == id).ExecuteDeleteAsync();
            return NoContent();
        }
    }
}
