using Microsoft.AspNetCore.Mvc;
using POS.API.Services;
using POS.API.ViewModels;

namespace POS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaleController : BaseController
    {
        private readonly SaleService _saleService;

        public SaleController(SaleService saleService)
        {
            _saleService = saleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSalesAsync()
        {
            var result = await _saleService.GetSalesAsync();
            return Execute<List<SaleViewModel>>(result);
        }

        [HttpGet("{id}", Name = "GetSaleById")]
        public async Task<IActionResult> GetSaleAsync(int id)
        {
            var result = await _saleService.GetSaleAsync(id);
            return Execute<SaleViewModel>(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSale(SaleViewModel sale)
        {
            var result = await _saleService.CreateSaleAsync(sale);
            return Execute<SaleViewModel>(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSale(int id, SaleViewModel sale)
        {
            var result = await _saleService.UpdateSaleAsync(id, sale);
            return Execute<SaleViewModel>(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSale(int id)
        {
            var result = await _saleService.DeleteSaleAsync(id);
            return Execute<SaleViewModel>(result);
        }
    }
}
