namespace GymManagement.ViewModels
{
  public class EditTrainerProfileViewModel
  {
    public int TrainerId { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public string? Specialization { get; set; }
    public DateTime? ExperienceStarted { get; set; }
  }
}
