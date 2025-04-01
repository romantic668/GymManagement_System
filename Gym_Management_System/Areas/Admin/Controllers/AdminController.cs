using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagement.Data;
using GymManagement.Models;

namespace GymManagement.Areas.Admin.Controllers
{
  [Area("Admin")]
  [Authorize(Roles = "Admin")] // Only Admin can access this controller
  public class AdminController : Controller
  {
    private readonly AppDbContext _dbContext;
    private readonly UserManager<User> _userManager;

    public AdminController(AppDbContext dbContext, UserManager<User> userManager)
    {
      _dbContext = dbContext;
      _userManager = userManager;
    }

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

    public IActionResult ManageMembers()
    {
      var members = _dbContext.Customers.ToList();
      ViewBag.Error = TempData["Error"];
      return View(members);
    }

    public IActionResult ManageTrainers()
    {
      var trainers = _dbContext.Trainers.ToList();
      ViewBag.Error = TempData["Error"];
      return View(trainers);
    }

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

    public IActionResult ManageGymBranches()
    {
      var branches = _dbContext.GymBranches.ToList();
      return View(branches);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string userId)
    {
      var user = await _userManager.FindByIdAsync(userId);
      if (user == null)
        return NotFound("User not found.");

      if (await _userManager.IsInRoleAsync(user, "Customer"))
      {
        var hasBookings = _dbContext.Bookings.Any(b => b.CustomerId == userId);
        var hasPayments = _dbContext.Payments.Any(p => p.CustomerId == userId);
        if (hasBookings || hasPayments)
        {
          TempData["Error"] = "Cannot delete a customer who has existing bookings or payments.";
          return RedirectToAction("ManageMembers");
        }
      }
      else if (await _userManager.IsInRoleAsync(user, "Trainer"))
      {
        var hasSessions = _dbContext.Sessions.Any(s => s.TrainerId == userId);
        if (hasSessions)
        {
          TempData["Error"] = "Cannot delete a trainer who is assigned to sessions.";
          return RedirectToAction("ManageTrainers");
        }
      }

      var result = await _userManager.DeleteAsync(user);
      if (!result.Succeeded)
        TempData["Error"] = "Failed to delete user.";

      bool isCustomer = await _userManager.IsInRoleAsync(user, "Customer");
      return RedirectToAction(isCustomer ? "ManageMembers" : "ManageTrainers");
    }
  }
}
