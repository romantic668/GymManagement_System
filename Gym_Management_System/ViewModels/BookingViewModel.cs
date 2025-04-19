public class BookingViewModel
{
    public required int BookingId { get; set; }
    public required string ClassName { get; set; }
    public required DateTime SessionDate { get; set; }
    public required string Status { get; set; }

    public string TrainerName { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // Yoga, HIIT etc.
    
}
