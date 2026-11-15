using System.ComponentModel.DataAnnotations;

namespace POS.Frontend.Models;

public class CategoryFormModel
{
    [Required(ErrorMessage = "Category name is required.")]
    [StringLength(50, ErrorMessage = "Category name cannot exceed 50 characters.")]
    public string Name { get; set; } = string.Empty;
}
