using System.ComponentModel.DataAnnotations;

namespace POS.Frontend.Models;

public class ProductFormModel
{
    [Required(ErrorMessage = "Product name is required.")]
    [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Product code is required.")]
    [StringLength(10, ErrorMessage = "Product code cannot exceed 10 characters.")]
    public string Code { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Category id must be greater than zero.")]
    public int? CategoryId { get; set; }

    [Range(typeof(decimal), "0", "99999999.99", ErrorMessage = "Price must be between 0 and 99999999.99.")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]
    public int StockQuantity { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Reorder level cannot be negative.")]
    public int ReorderLevel { get; set; }
}
