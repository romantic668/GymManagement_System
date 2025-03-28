using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GymManagement.Models;

namespace GymManagement.Data
{
  public class AppDbContext : IdentityDbContext<User>
  {
    private readonly IConfiguration _configuration;

    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
        : base(options)
    {
      _configuration = configuration;
    }

    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<GymBranch> GymBranches { get; set; }
    public DbSet<GymClass> GymClasses { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Receptionist> Receptionists { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Trainer> Trainers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlite(_configuration.GetConnectionString("DefaultConnection"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<GymBranch>().HasKey(g => g.BranchId);

      modelBuilder.Entity<GymBranch>().HasData(
          new GymBranch
          {
            BranchId = 1,
            BranchName = "Downtown Gym",
            Address = "123 Main St",
            ContactNumber = "123-456-7890"
          },
          new GymBranch
          {
            BranchId = 2,
            BranchName = "Uptown Gym",
            Address = "456 High St",
            ContactNumber = "987-654-3210"
          }
      );

      modelBuilder.Entity<Room>().HasData(
          new Room
          {
            RoomId = 1,
            RoomName = "Yoga Room",
            Capacity = 20,
            IsAvailable = true,
            BranchId = 1
          },
          new Room
          {
            RoomId = 2,
            RoomName = "Weightlifting Room",
            Capacity = 30,
            IsAvailable = true,
            BranchId = 2
          }
      );

      // ✅ GymClass 数据移到 SeedData.cs 中处理，因 TrainerId 需数据库用户支持
    }
  }
}
