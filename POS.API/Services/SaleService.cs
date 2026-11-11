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

    public async Task<Result<List<SaleViewModel>>> GetSalesAsync()
    {
        Result<List<SaleViewModel>> model;
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
        model = Result<List<SaleViewModel>>.Success("Sales retrieved successfully", list);
        return model;
    }

    public async Task<Result<SaleViewModel>> GetSaleAsync(int id)
    {
        Result<SaleViewModel> model;
        var entity = await _db.Sales
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
        if (entity is null)
        {
            model = Result<SaleViewModel>.ValidationError("Sale not found");
            goto Response;
        }
        model = Result<SaleViewModel>.Success("Sale retrieved successfully", entity);

        Response:
        return model;
    }

    public async Task<Result<SaleViewModel>> CreateSaleAsync(SaleViewModel sale)
    {
        Result<SaleViewModel> model;
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

        model = Result<SaleViewModel>.Success("Sale created successfully", sale);
        return model;
    }

    public async Task<Result<SaleViewModel>> UpdateSaleAsync(int id, SaleViewModel sale)
    {
        Result<SaleViewModel> model;
        var entity = await _db.Sales.FirstOrDefaultAsync(x => x.SaleId == id);
        if (entity is null)
        {
            model = Result<SaleViewModel>.ValidationError("Sale not found");
            goto Response;
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
        model = Result<SaleViewModel>.Success("Sale updated successfully", sale);

    Response:
        return model;
    }

    public async Task<Result<SaleViewModel>> DeleteSaleAsync(int id)
    {
        Result<SaleViewModel> model;
        await _db.SaleDetails
            .Where(x => x.SaleId == id)
            .ExecuteDeleteAsync();
        await _db.Sales
            .Where(x => x.SaleId == id)
            .ExecuteDeleteAsync();
        model = Result<SaleViewModel>.Success();
        return model;
    }
}
