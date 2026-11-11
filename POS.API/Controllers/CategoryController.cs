using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using POS.API.Services;
using POS.API.ViewModels;

namespace POS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : BaseController
    {
        private readonly CategoryService _categoryService;

        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoriesAsync()
        {
            var result = await _categoryService.GetCategoriesAsync();
            return Execute<List<CategoryViewModel>>(result);
        }

        [HttpGet("{id}", Name = "GetCategoryById")]
        public async Task<IActionResult> GetCategoryAsync(int id)
        {
            var cat = await _categoryService.GetCategoryAsync(id);
            return Ok(cat);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryViewModel category)
        {
            var result = await _categoryService.CreateCategoryAsync(category);
            return Execute<CategoryViewModel>(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryViewModel category)
        {
            var result = await _categoryService.UpdateCategoryAsync(id, category);
            return Execute<CategoryViewModel>(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            return Execute<CategoryViewModel>(result);
        }
    }
}
