// Admin.cs
namespace GymManagement.Models;
public class Admin : User
{
  public required ICollection<Trainer> Trainers { get; set; }
  public required ICollection<GymClass> GymClasses { get; set; }
  public required ICollection<Room> Rooms { get; set; }
  public required ICollection<Session> Sessions { get; set; }

}
