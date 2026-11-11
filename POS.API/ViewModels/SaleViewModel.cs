namespace POS.API.ViewModels
{
    public class SaleViewModel
    {
        public int SaleId { get; set; }

        public DateTime Date { get; set; }

        public string InvoiceNo { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public List<SaleDetailViewModel> SaleDetails { get; set; } = new();
    }
}
