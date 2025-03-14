using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using GymManagement.Models;

namespace GymManagement.Data
{
  public class AppDbContext : DbContext
  {
    private readonly IConfiguration _configuration;

    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration) : base(options)
    {
      _configuration = configuration;
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<LibraryBranch> LibraryBranches { get; set; }
    public DbSet<BookInventory> BookInventories { get; set; }
    public DbSet<BorrowedBook> BorrowedBooks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlite(_configuration.GetConnectionString("DefaultConnection"));
    }
  }
}
