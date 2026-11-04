using System;
using System.Collections.Generic;

namespace POS.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? ProductCode { get; set; }

    public int? CatId { get; set; }

    public decimal Price { get; set; }

    public virtual Category? Cat { get; set; }

    public virtual ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
}
