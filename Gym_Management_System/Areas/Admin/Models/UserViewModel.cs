using Microsoft.AspNetCore.Identity;
using GymManagement.Models; // ✅ 添加这一行

namespace GymManagement.Areas.Admin.Models
{
    public class UserViewModel
    {
        public IEnumerable<User> Users { get; set; } = null!;
        public IEnumerable<IdentityRole> Roles { get; set; } = null!;
    }
}
