using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace GymManagement.Models
{
  public class User : IdentityUser
  {
    [Required]
    public string Name { get; set; } = "";
    public DateTime JoinDate { get; set; } = DateTime.UtcNow;
  }
}
