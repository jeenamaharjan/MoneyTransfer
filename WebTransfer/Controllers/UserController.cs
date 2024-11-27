using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public class UserController : Controller
{
    private readonly AppDbContext _context;
    private readonly string _exchangeRateApiUrl = "https://www.nrb.org.np/api/forex/v1/rates?page=1&per_page=5&from=2024-06-12&to=2024-06-12"; 

    public UserController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Signup() => View();

    [HttpPost]
    public IActionResult Signup(string username, string password)
    {
        if (_context.Users.Any(u => u.Username == username))
        {
            ModelState.AddModelError("", "Username already exists");
            return View();
        }

        var hashedPassword = HashPassword(password);
        _context.Users.Add(new User { Username = username, Password = hashedPassword });
        _context.SaveChanges();

        return RedirectToAction("Login");
    }

    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var hashedPassword = HashPassword(password);
        var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == hashedPassword);

        if (user == null)
        {
            ModelState.AddModelError("", "Invalid login attempt");
            return View();
        }

        // Fetch exchange rate after successful login
        var exchangeRate = await GetExchangeRateAsync();

        // Pass exchange rate to the view
        ViewBag.ExchangeRate = exchangeRate;

        // Return to the ExchangeRate/Index view
      
        return RedirectToAction("Index", "ExchangeRate");
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }

    
    private async Task<decimal> GetExchangeRateAsync()
    {
        using (var client = new HttpClient())
        {
            var response = await client.GetStringAsync(_exchangeRateApiUrl);
            dynamic exchangeRates = JsonConvert.DeserializeObject(response);

            
            var rates = exchangeRates.rates as IEnumerable<dynamic>;
            var exchangeRate = rates?.FirstOrDefault(rate => rate.currency == "MYR")?.rate;

            return exchangeRate != null ? exchangeRate : 0;
        }
    }
}
