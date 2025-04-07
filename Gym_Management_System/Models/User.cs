using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace GymManagement.Models
{
  public class User : IdentityUser
  {
    [Required]
    public string Name { get; set; } = "";

    public DateTime JoinDate { get; set; } = DateTime.UtcNow;

    // ✅ 新增：Date of Birth（可以为空）
    [DataType(DataType.Date)]
    public DateTime? DOB { get; set; }


    [NotMapped] // 防止 EF 把 RoleNames 映射到数据库
    public IList<string>? RoleNames { get; set; }
  }
}
