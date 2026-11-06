using Microsoft.EntityFrameworkCore;
using POS.API.ViewModels;
using POS.Data;
using POS.Models;

namespace POS.API.Services;

public class SaleService
{
    private readonly AppDbContext _db;

    public SaleService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<SaleViewModel>> GetSalesAsync()
    {
        return await _db.Sales.AsNoTracking()
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
    }

    public async Task<SaleViewModel?> GetSaleAsync(int id)
    {
        return await _db.Sales
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
    }

    public async Task<SaleViewModel> CreateSaleAsync(SaleViewModel sale)
    {
        var entity = new Sale
        {
            InvoiceNo = sale.InvoiceNo,
            Date = sale.Date,
            Amount = sale.Amount,
            SaleDetails = sale.SaleDetails
                .Select(d => new SaleDetail
                {
                    Lno = d.Lno,
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    Price = d.Price
                })
                .ToList()
        };

        await _db.Sales.AddAsync(entity);
        await _db.SaveChangesAsync();

        sale.SaleId = entity.SaleId;
        foreach (var detail in sale.SaleDetails)
        {
            detail.SaleId = entity.SaleId;
        }

        return sale;
    }

    public async Task<SaleViewModel?> UpdateSaleAsync(int id, SaleViewModel sale)
    {
        var entity = await _db.Sales.FirstOrDefaultAsync(x => x.SaleId == id);
        if (entity is null)
        {
            return null;
        }

        entity.Date = sale.Date;
        entity.InvoiceNo = sale.InvoiceNo;
        entity.Amount = sale.Amount;
        await _db.SaveChangesAsync();

        await _db.SaleDetails
            .Where(x => x.SaleId == id)
            .ExecuteDeleteAsync();

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
        return sale;
    }

    public async Task DeleteSaleAsync(int id)
    {
        await _db.SaleDetails
            .Where(x => x.SaleId == id)
            .ExecuteDeleteAsync();
        await _db.Sales
            .Where(x => x.SaleId == id)
            .ExecuteDeleteAsync();
    }
}
