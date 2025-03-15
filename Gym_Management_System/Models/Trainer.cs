// Trainer.cs
namespace GymManagement.Models;
public class Trainer : User
{
  public required string Specialization { get; set; }
  public required DateTime ExperienceStarted { get; set; }
  public required int BranchId { get; set; }
  public GymBranch? GymBranch { get; set; }
  public required ICollection<GymClass> GymClasses { get; set; } // 1 Trainer teaches multiple GymClasses
  public required ICollection<Session> Sessions { get; set; } // One Trainer teaches multiple Sessions
}
