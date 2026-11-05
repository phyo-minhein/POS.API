namespace POS.API.ViewModels
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string? ProductCode { get; set; }

        public int? CatId { get; set; }

        public decimal Price { get; set; }
    }
}
