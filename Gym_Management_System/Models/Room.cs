// Room.cs
namespace GymManagement.Models;
public class Room
{
  public required int RoomId { get; set; }
  public string? RoomName { get; set; }
  public required int Capacity { get; set; }
  public required bool IsAvailable { get; set; }
  public required int BranchId { get; set; }  // fk
  public GymBranch? GymBranch { get; set; }
}
