using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebTransfer.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Linq;
using System.Collections.Generic;
using System;

public class TransferMoneyController : Controller
{
    private readonly string _exchangeRateApiUrl = "https://www.nrb.org.np/api/forex/v1/rates?page=1&per_page=5&from=2024-06-12&to=2024-06-12";
    private readonly AppDbContext _dbContext; 

    public TransferMoneyController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Action to show transfer form
    public IActionResult Transfer()
    {
        return View(new TransferMoneyModel());
    }

    // Action to handle transfer
    [HttpPost]
    public async Task<IActionResult> Transfer(TransferMoneyModel model)
    {
        if (!ModelState.IsValid)
        {
            Console.WriteLine("Model validation failed");
            return View(model); 
        }
        Console.WriteLine($"Received amount: {model.Amount}");
        var exchangeRate = await GetExchangeRateAsync(model.FromCurrency, model.ToCurrency);

        if (exchangeRate > 0)
        {
            model.ConvertedAmount = model.Amount * exchangeRate;

            // Save the transaction to the database
            await SaveTransaction(model);
            Console.WriteLine("Transaction saved");

            return View("TransferConfirmation", model);
        }

        ModelState.AddModelError("", "Failed to retrieve exchange rate. Please try again.");
        return View(model);
    }

    
    private async Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency)
    {
        try
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync(_exchangeRateApiUrl);
                dynamic exchangeRates = JsonConvert.DeserializeObject(response);

                var exchangeRate = (exchangeRates.rates as IEnumerable<dynamic>)?
                    .FirstOrDefault(rate => rate.currency == fromCurrency)?.rate;

                return exchangeRate != null ? exchangeRate : 0;
            }
        }
        catch (Exception ex)
        {
           
            Console.WriteLine($"Error fetching exchange rate: {ex.Message}");
            return 0;
        }
    }

    private async Task SaveTransaction(TransferMoneyModel model)
    {
        var transaction = new Transaction
        {
            Amount = model.Amount,
            FromCurrency = model.FromCurrency,
            ToCurrency = model.ToCurrency,
            ConvertedAmount = model.ConvertedAmount,
            TransactionDate = DateTime.Now
        };

        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync();
    }
}
