using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class ExchangeRateController : Controller
{
    private readonly string _apiUrl = "https://www.nrb.org.np/api/forex/v1/rates?page=1&per_page=5&from=2024-06-12&to=2024-06-12";

    // Action to get exchange rates
    public async Task<IActionResult> Index()
    {
        var exchangeRates = await GetExchangeRatesAsync();
        return View(exchangeRates);
    }

    
    private async Task<object> GetExchangeRatesAsync()
    {
        using (var client = new HttpClient())
        {
            var response = await client.GetStringAsync(_apiUrl);
            var exchangeRates = JsonConvert.DeserializeObject(response);
            return exchangeRates;
        }
    }
}
