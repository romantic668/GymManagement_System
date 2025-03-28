using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManagement.Models
{
  public class Session
  {
    [Key]
    public int SessionId { get; set; }

    [Required]
    public DateTime SessionDateTime { get; set; }

    [Required]
    public int Capacity { get; set; }

    // 🔸 GymClass FK
    [Required]
    public int GymClassId { get; set; }

    [ForeignKey("GymClassId")]
    public GymClass GymClass { get; set; } = null!;

    // 🔸 Room FK
    [Required]
    public int RoomId { get; set; }

    [ForeignKey("RoomId")]
    public Room Room { get; set; } = null!;

    // 🔸 Trainer FK (string type from IdentityUser)
    [Required]
    public string TrainerId { get; set; } = string.Empty;

    [ForeignKey("TrainerId")]
    public Trainer Trainer { get; set; } = null!;

    // 🔸 Bookings
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    // 🔸 Optional Receptionist FK
    public string? ReceptionistId { get; set; }

    [ForeignKey("ReceptionistId")]
    public Receptionist? Receptionist { get; set; }
  }
}

