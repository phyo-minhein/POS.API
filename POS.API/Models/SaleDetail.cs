using System;
using System.Collections.Generic;

namespace POS.Models;

public partial class SaleDetail
{
    public int SaleId { get; set; }

    public int Lno { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Sale Sale { get; set; } = null!;
}
