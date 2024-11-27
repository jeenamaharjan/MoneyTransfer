using Microsoft.EntityFrameworkCore;
using WebTransfer.Models;


public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
	public DbSet<Transaction> Transactions { get; set; }

	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}
