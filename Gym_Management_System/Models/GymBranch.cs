// GymBranch.cs
namespace GymManagement.Models;
public class GymBranch
{
  public required int BranchId { get; set; }
  public required string BranchName { get; set; }
  public string? Address { get; set; }
  public string? ContactNumber { get; set; }
  public required ICollection<Trainer> Trainers { get; set; }  // 1..*
  public required ICollection<Receptionist> Receptionists { get; set; }  // 1..*
  public required ICollection<Room> Rooms { get; set; } // 1..*
}
