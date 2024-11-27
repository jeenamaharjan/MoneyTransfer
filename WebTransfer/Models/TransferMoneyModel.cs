using System.ComponentModel.DataAnnotations;
namespace WebTransfer.Models
{
    public class TransferMoneyModel
    {
        public decimal Amount { get; set; }
        public string FromCurrency { get; set; } = "MYR"; // Malaysia Ringgit
        public string ToCurrency { get; set; } = "NPR"; // Nepalese Rupee
        public decimal ConvertedAmount { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
    }
}
