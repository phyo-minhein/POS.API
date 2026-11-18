namespace POS.API.Dtos.Products;

public class ProductQueryDto
{
    public string? Search { get; set; }

    public int? CategoryId { get; set; }

    public bool LowStockOnly { get; set; }

    public string SortBy { get; set; } = "name";

    public bool SortDescending { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}
