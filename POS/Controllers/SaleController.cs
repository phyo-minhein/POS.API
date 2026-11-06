using Microsoft.AspNetCore.Mvc;
using POS.API.Services;
using POS.API.ViewModels;

namespace POS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaleController : Controller
    {
        private readonly SaleService _saleService;

        public SaleController(SaleService saleService)
        {
            _saleService = saleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSalesAsync()
        {
            var list = await _saleService.GetSalesAsync();
            return Ok(list);
        }

        [HttpGet("{id}", Name = "GetSaleById")]
        public async Task<IActionResult> GetSaleAsync(int id)
        {
            var sale = await _saleService.GetSaleAsync(id);

            if (sale is null)
            {
                return NotFound();
            }

            return Ok(sale);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSale(SaleViewModel sale)
        {
            sale = await _saleService.CreateSaleAsync(sale);
            return Ok(sale);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSale(int id, SaleViewModel sale)
        {
            var updatedSale = await _saleService.UpdateSaleAsync(id, sale);
            if (updatedSale is null)
            {
                return NotFound();
            }
            return Ok(updatedSale);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSale(int id)
        {
            await _saleService.DeleteSaleAsync(id);
            return NoContent();
        }
    }
}
