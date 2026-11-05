using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS.API.ViewModels;
using POS.Data;
using POS.Models;

namespace POS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaleController : Controller
    {
        private readonly AppDbContext _db;

        public SaleController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetSalesAsync()
        {
            var list = await _db.Sales.AsNoTracking()
                                .Select(x => new SaleViewModel
                                {
                                    SaleId = x.SaleId,
                                    Date = x.Date,
                                    InvoiceNo = x.InvoiceNo,
                                    Amount = x.Amount,
                                    SaleDetails = x.SaleDetails
                                        .Select(d => new SaleDetailViewModel
                                        {
                                            SaleId = d.SaleId,
                                            Lno = d.Lno,
                                            ProductId = d.ProductId,
                                            Quantity = d.Quantity,
                                            Price = d.Price
                                        })
                                        .ToList()
                                })
                                .ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id}", Name = "GetSaleById")]
        public async Task<IActionResult> GetSaleAsync(int id)
        {
            var sale = await _db.Sales
                .AsNoTracking()
                .Where(x => x.SaleId == id)
                .Select(x => new SaleViewModel
                {
                    SaleId = x.SaleId,
                    Date = x.Date,
                    InvoiceNo = x.InvoiceNo,
                    Amount = x.Amount,
                    SaleDetails = x.SaleDetails
                        .Select(d => new SaleDetailViewModel
                        {
                            SaleId = d.SaleId,
                            Lno = d.Lno,
                            ProductId = d.ProductId,
                            Quantity = d.Quantity,
                            Price = d.Price
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (sale is null)
            {
                return NotFound();
            }

            return Ok(sale);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSale(SaleViewModel sale)
        {
            var entity = new Sale
            {
                InvoiceNo = sale.InvoiceNo,
                Date = sale.Date,
                Amount = sale.Amount,
                SaleDetails = sale.SaleDetails.Select(
                    d => new SaleDetail
                    {
                        Lno = d.Lno,
                        ProductId = d.ProductId,
                        Quantity = d.Quantity,
                        Price = d.Price
                    }).ToList()
            };
            await _db.Sales.AddAsync(entity);
            await _db.SaveChangesAsync();

            sale.SaleId = entity.SaleId;
            foreach (var detail in sale.SaleDetails)
            {
                detail.SaleId = entity.SaleId;
            }
            return Ok(sale);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSale(int id, SaleViewModel sale)
        {
            var entity = await _db.Sales.FirstOrDefaultAsync(x => x.SaleId == id);
            if (entity is null)
            {
                return NotFound();
            }
            entity.Date = sale.Date;
            entity.InvoiceNo = sale.InvoiceNo;
            entity.Amount = sale.Amount;
            await _db.SaveChangesAsync();

            await _db.SaleDetails.Where(x => x.SaleId == id).ExecuteDeleteAsync();
            var details = sale.SaleDetails
                .Select(d => new SaleDetail
                {
                    SaleId = id,
                    Lno = d.Lno,
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    Price = d.Price
                })
                .ToList();
            await _db.SaleDetails.AddRangeAsync(details);
            await _db.SaveChangesAsync();

            sale.SaleId = id;
            return Ok(sale);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSale(int id)
        {
            await _db.SaleDetails.Where(x => x.SaleId == id).ExecuteDeleteAsync();
            await _db.Sales.Where(x => x.SaleId == id).ExecuteDeleteAsync();
            return NoContent();
        }
    }
}
