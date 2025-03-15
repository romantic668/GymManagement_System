// User.cs
namespace GymManagement.Models;
public class User
{
  public int Id { get; set; }
  public required string Name { get; set; }
  public string? Email { get; set; }
  public required string Password { get; set; }
  public required Role Role { get; set; }   // Role: Admin, Receptionist, Trainer, Customer
  public required DateTime JoinDate { get; set; }
}
public enum Role
{
  Admin,
  Receptionist,
  Trainer,
  Customer
}