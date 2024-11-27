using Microsoft.EntityFrameworkCore;
using WebTransfer.Models;


namespace WebTransfer.Models
{
	public class Transaction
	{
		public int Id { get; set; }
		public decimal Amount { get; set; }
		public string FromCurrency { get; set; }
		public string ToCurrency { get; set; }
		public decimal ConvertedAmount { get; set; }
		public DateTime TransactionDate { get; set; }
	}
}
