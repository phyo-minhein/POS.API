using Microsoft.AspNetCore.Mvc;
using POS.API.Dtos.Categories;
using POS.API.Services;

namespace POS.API.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoryController : BaseController
{
    private readonly CategoryService _categoryService;

    public CategoryController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetCategoriesAsync(cancellationToken);
        return Execute(result);
    }

    [HttpGet("{id:int}", Name = "GetCategoryById")]
    public async Task<IActionResult> GetCategoryAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetCategoryAsync(id, cancellationToken);
        return Execute(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategoryAsync(
        CreateCategoryDto category,
        CancellationToken cancellationToken)
    {
        var result = await _categoryService.CreateCategoryAsync(category, cancellationToken);

        if (!result.IsSuccess)
        {
            return Execute(result);
        }

        return CreatedAtRoute(
            "GetCategoryById",
            new { id = result.Data!.Id },
            result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCategoryAsync(
        int id,
        UpdateCategoryDto category,
        CancellationToken cancellationToken)
    {
        var result = await _categoryService.UpdateCategoryAsync(id, category, cancellationToken);
        return Execute(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategoryAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _categoryService.DeleteCategoryAsync(id, cancellationToken);
        return Execute(result);
    }
}
