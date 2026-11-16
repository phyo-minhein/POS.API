using System;
using System.Collections.Generic;

namespace POS.Models;

public class Category
{
    public int CatId { get; set; }

    public string CatName { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
