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

    public DbSet<Admin> Admins { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<GymBranch> GymBranches { get; set; }
    public DbSet<GymClass> GymClasses { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Receptionist> Receptionists { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Trainer> Trainers { get; set; }
    public DbSet<User> Users { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlite(_configuration.GetConnectionString("DefaultConnection"));
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<GymBranch>()
          .HasKey(g => g.BranchId);
    }

  }
}
