// Receptionist.cs
namespace GymManagement.Models;
public class Receptionist : User
{
  public required string Responsibilities { get; set; }
  public string? Notes { get; set; }
  public required bool IsAvailable { get; set; }
  public required int BranchId { get; set; }
  public GymBranch? GymBranch { get; set; }
}
