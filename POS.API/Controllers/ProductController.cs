using Microsoft.AspNetCore.Mvc;
using POS.API.Services;
using POS.API.ViewModels;

namespace POS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            return Execute<List<ProductViewModel>>(result);
        }

        [HttpGet("{id}", Name = "GetProductById")]
        public async Task<IActionResult> GetProductAsync(int id)
        {
            var result = await _productService.GetProductAsync(id);
            return Execute<ProductViewModel>(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductViewModel product)
        {
            var result = await _productService.CreateProductAsync(product);
            return Execute<ProductViewModel>(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductViewModel product)
        {
            var result = await _productService.UpdateProductAsync(id, product);
            return Execute<ProductViewModel>(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            return Execute<ProductViewModel>(result);
        }
    }
}
