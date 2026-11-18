using System.ComponentModel.DataAnnotations;

namespace POS.API.Dtos.Products;

public class ProductQueryDto
{
    [StringLength(100, ErrorMessage = "Search cannot exceed 100 characters.")]
    public string? Search { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Category id must be greater than zero.")]
    public int? CategoryId { get; set; }

    public bool LowStockOnly { get; set; }

    [StringLength(20, ErrorMessage = "Sort field cannot exceed 20 characters.")]
    public string SortBy { get; set; } = "name";

    public bool SortDescending { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than zero.")]
    public int Page { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100.")]
    public int PageSize { get; set; } = 10;
}
