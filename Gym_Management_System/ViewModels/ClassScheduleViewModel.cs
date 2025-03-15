public class ClassScheduleViewModel
{
  public required int SessionId { get; set; }
  public required string ClassName { get; set; }
  public required string TrainerName { get; set; }
  public required string RoomName { get; set; }
  public DateTime? SessionDateTime { get; set; }
  public required int Capacity { get; set; }
  public required int BookedCount { get; set; }
}
