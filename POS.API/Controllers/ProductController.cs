using Microsoft.AspNetCore.Mvc;
using POS.API.Dtos.Products;
using POS.API.Services;

namespace POS.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController : BaseController
{
    private readonly ProductService _productService;

    public ProductController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProductsAsync()
    {
        var result = await _productService.GetProductsAsync();
        return Execute(result);
    }

    [HttpGet("{id:int}", Name = "GetProductById")]
    public async Task<IActionResult> GetProductAsync(int id)
    {
        var result = await _productService.GetProductAsync(id);
        return Execute(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProductAsync(CreateProductDto product)
    {
        var result = await _productService.CreateProductAsync(product);
        return Execute(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProductAsync(int id, UpdateProductDto product)
    {
        var result = await _productService.UpdateProductAsync(id, product);
        return Execute(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProductAsync(int id)
    {
        var result = await _productService.DeleteProductAsync(id);
        return Execute(result);
    }
}
