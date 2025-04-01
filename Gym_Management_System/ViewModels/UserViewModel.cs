using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using GymManagement.Models;

namespace GymManagement.ViewModels
{
  public class UserViewModel
  {
    public IEnumerable<User>? Users { get; set; }
    public IEnumerable<IdentityRole>? Roles { get; set; }
  }
}
