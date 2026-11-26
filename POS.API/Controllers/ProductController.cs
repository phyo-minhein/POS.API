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
    public async Task<IActionResult> GetProductsPagingAsync(
        [FromQuery] ProductParamModel param,
        CancellationToken cancellationToken)
    {
        var result = await _productService.GetProductsPagingAsync(param, cancellationToken);
        return Execute(result);
    }

    [HttpGet("{id:int}", Name = "GetProductById")]
    public async Task<IActionResult> GetProductAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _productService.GetProductAsync(id, cancellationToken);
        return Execute(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProductAsync(
        CreateProductRequestModel product,
        CancellationToken cancellationToken)
    {
        var result = await _productService.CreateProductAsync(product, cancellationToken);

        if (!result.IsSuccess)
        {
            return Execute(result);
        }

        return CreatedAtRoute(
            "GetProductById",
            new { id = result.Data!.Id },
            result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProductAsync(
        int id,
        UpdateProductRequestModel product,
        CancellationToken cancellationToken)
    {
        var result = await _productService.UpdateProductAsync(id, product, cancellationToken);
        return Execute(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProductAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _productService.DeleteProductAsync(id, cancellationToken);
        return Execute(result);
    }
}
