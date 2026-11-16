using System;
using System.Collections.Generic;

namespace POS.Models;

public class Sale
{
    public int SaleId { get; set; }

    public DateTime Date { get; set; }

    public string InvoiceNo { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
}
