using System;
using System.Collections.Generic;

namespace POS.Models;

public partial class Sale
{
    public int SaleId { get; set; }

    public DateTime Date { get; set; }

    public string InvoiceNo { get; set; } = null!;

    public decimal Amount { get; set; }

    public virtual ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
}
