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

      modelBuilder.Entity<GymBranch>().HasKey(g => g.BranchId);

      modelBuilder.Entity<GymBranch>().HasData(
           new GymBranch
           {
             BranchId = 1,
             BranchName = "Downtown Gym",
             Address = "123 Main St",
             ContactNumber = "123-456-7890",
             Trainers = new List<Trainer>(),
             Receptionists = new List<Receptionist>(),
             Rooms = new List<Room>()
           },
           new GymBranch
           {
             BranchId = 2,
             BranchName = "Uptown Gym",
             Address = "456 High St",
             ContactNumber = "987-654-3210",
             Trainers = new List<Trainer>(),
             Receptionists = new List<Receptionist>(),
             Rooms = new List<Room>()
           }
       );

      //  User (Admin)

      modelBuilder.Entity<Admin>().HasData(
        new Admin
        {
          Id = 1,
          Name = "Admin User",
          Email = "admin@example.com",
          Password = "Admin@123",
          Role = Role.Admin,
          JoinDate = new DateTime(2024, 1, 1),

          Sessions = new List<Session>(),
          Trainers = new List<Trainer>(),
          GymClasses = new List<GymClass>(),
          Rooms = new List<Room>()
        }
      );


      //  Customer
      modelBuilder.Entity<Customer>().HasData(
          new Customer
          {
            Id = 2,
            Name = "Jane Doe",
            Email = "jane@example.com",
            Password = "Customer@123",
            Role = Role.Customer,
            JoinDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            SubscriptionDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            MembershipType = "Premium",
            Bookings = new List<Booking>(),
            Payments = new List<Payment>()
          }
      );

      //  Trainer
      modelBuilder.Entity<Trainer>().HasData(
          new Trainer
          {
            Id = 3,
            Name = "John Trainer",
            Email = "johntrainer@example.com",
            Password = "Trainer@123",
            Role = Role.Trainer,
            JoinDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Specialization = "Yoga",
            ExperienceStarted = new DateTime(2015, 1, 1),
            BranchId = 1,
            GymBranch = null,
            GymClasses = new List<GymClass>(),
            Sessions = new List<Session>()
          }
      );

      //  Receptionist
      modelBuilder.Entity<Receptionist>().HasData(
          new Receptionist
          {
            Id = 4,
            Name = "Mike Receptionist",
            Email = "mike@example.com",
            Password = "Receptionist@123",
            Role = Role.Receptionist,
            JoinDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            BranchId = 1,
            IsAvailable = true,
            Responsibilities = "Front Desk Management",
            GymBranch = null
          }
      );

      //  Rooms
      modelBuilder.Entity<Room>().HasData(
          new Room
          {
            RoomId = 1,
            RoomName = "Yoga Room",
            Capacity = 20,
            IsAvailable = true,
            BranchId = 1,
          },
          new Room
          {
            RoomId = 2,
            RoomName = "Weightlifting Room",
            Capacity = 30,
            IsAvailable = true,
            BranchId = 2,
          }
      );

      //  Gym Classes
      modelBuilder.Entity<GymClass>().HasData(
          new GymClass
          {
            GymClassId = 1,
            ClassName = "Beginner Yoga",
            AvailableTime = new DateTime(2025, 3, 15, 10, 0, 0),
            Duration = 60,
            TrainerId = 3,
            Trainer = null,
            Sessions = new List<Session>()
          }
      );
    }
  }


}
