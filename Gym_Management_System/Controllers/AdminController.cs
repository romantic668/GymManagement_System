// AdminController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagement.Data;
using GymManagement.Models;

[Authorize(Roles = "Admin")]  // 仅管理员可访问
public class AdminController : Controller
{
  private readonly AppDbContext _dbContext;

  public AdminController(AppDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  // 🔹 管理员仪表盘
  public IActionResult Dashboard()
  {
    var dashboardData = new AdminDashboardViewModel
    {
      TotalMembers = _dbContext.Customers.Count(),
      TotalTrainers = _dbContext.Trainers.Count(),
      TotalSessions = _dbContext.Sessions.Count(),
      TotalBranches = _dbContext.GymBranches.Count()
    };

    return View(dashboardData);
  }

  // 🔹 管理所有会员
  public IActionResult ManageMembers()
  {
    var members = _dbContext.Customers.ToList();
    return View(members);
  }

  // 🔹 管理所有教练
  public IActionResult ManageTrainers()
  {
    var trainers = _dbContext.Trainers.ToList();
    return View(trainers);
  }

  // 🔹 管理所有课程安排
  public IActionResult ManageSessions()
  {
    var sessions = _dbContext.Sessions
        .Include(s => s.GymClass)
        .Include(s => s.Trainer)
        .Include(s => s.Room)
        .OrderBy(s => s.SessionDateTime)
        .ToList();

    return View(sessions);
  }

  // 🔹 管理所有健身房分店
  public IActionResult ManageGymBranches()
  {
    var branches = _dbContext.GymBranches.ToList();
    return View(branches);
  }

  // 🔹 删除会员或教练
  [HttpPost]
  [ValidateAntiForgeryToken]
  public IActionResult DeleteUser(int userId)
  {
    var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);

    if (user == null)
    {
      return NotFound("User not found.");
    }

    // 确保删除前不影响系统完整性
    if (user.Role == Role.Customer)
    {
      var hasBookings = _dbContext.Bookings.Any(b => b.CustomerId == userId);
      var hasPayments = _dbContext.Payments.Any(p => p.CustomerId == userId);
      if (hasBookings || hasPayments)
      {
        ViewBag.Error = "Cannot delete a customer with existing bookings or payments.";
        return RedirectToAction("ManageMembers");
      }
    }
    else if (user.Role == Role.Trainer)
    {
      var hasSessions = _dbContext.Sessions.Any(s => s.TrainerId == userId);
      if (hasSessions)
      {
        ViewBag.Error = "Cannot delete a trainer who is assigned to sessions.";
        return RedirectToAction("ManageTrainers");
      }
    }

    _dbContext.Users.Remove(user);
    _dbContext.SaveChanges();

    return RedirectToAction(user.Role == Role.Customer ? "ManageMembers" : "ManageTrainers");
  }
}
