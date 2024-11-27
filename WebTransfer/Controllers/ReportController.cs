using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebTransfer.Controllers
{
	public class ReportController : Controller
	{ 
	private readonly AppDbContext _context;

	public ReportController(AppDbContext context)
	{
		_context = context;
	}

	// Action to display the report form
	public IActionResult GenerateReport()
	{
		return View();
	}

	// Action to generate the report based on date range
	[HttpPost]
	public IActionResult GenerateReport(DateTime startDate, DateTime endDate)
	{
		var transactions = _context.Transactions
			.Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
			.ToList();

		return View("ReportResult", transactions);
	}
}
}
