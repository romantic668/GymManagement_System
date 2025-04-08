using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymManagement.Data;
using GymManagement.Models;
using GymManagement.Areas.Admin.Models;

namespace GymManagement.Areas.Admin.Controllers
{
  [Area("Admin")]
  [Authorize(Roles = "Admin")]
  public class AdminController : Controller
  {
    private readonly AppDbContext _dbContext;
    private readonly UserManager<User> _userManager;

    public AdminController(AppDbContext dbContext, UserManager<User> userManager)
    {
      _dbContext = dbContext;
      _userManager = userManager;
    }

    public async Task<IActionResult> Dashboard()
    {
      var today = DateTime.Today;

      var sessions = await _dbContext.Sessions
          .Include(s => s.Trainer).ThenInclude(t => t.GymBranch)
          .Include(s => s.Room)
          .Include(s => s.GymClass)
          .Where(s => s.SessionDateTime.Date >= today)
          .ToListAsync();

      var sessionsByDate = sessions
          .GroupBy(s => s.SessionDateTime.Date)
          .OrderBy(g => g.Key)
          .ToDictionary(g => g.Key, g => g.ToList());

      var viewModel = new AdminDashboardViewModel
      {
        TotalSessions = await _dbContext.Sessions.CountAsync(),
        TotalMembers = await _dbContext.Users.OfType<Customer>().CountAsync(),
        TotalTrainers = await _dbContext.Users.OfType<Trainer>().CountAsync(),
        TotalBranches = await _dbContext.GymBranches.CountAsync(),
        UpcomingSessionsByDate = sessionsByDate
      };

      return View(viewModel);
    }

    [HttpGet]
    public IActionResult CreateSession()
    {
      var vm = new CreateSessionViewModel
      {
        GymClassList = _dbContext.GymClasses
              .Select(c => new SelectListItem { Text = c.ClassName, Value = c.GymClassId.ToString() })
              .ToList(),
        TrainerList = _dbContext.Users
              .OfType<Trainer>()
              .Select(t => new SelectListItem { Text = t.Name, Value = t.Id })
              .ToList(),
        RoomList = _dbContext.Rooms
              .Select(r => new SelectListItem { Text = r.RoomName, Value = r.RoomId.ToString() })
              .ToList(),
        BranchName = "Metro Branch"
      };
      return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSession(CreateSessionViewModel vm)
    {
      if (!ModelState.IsValid)
      {
        vm.GymClassList = _dbContext.GymClasses
            .Select(c => new SelectListItem { Text = c.ClassName, Value = c.GymClassId.ToString() })
            .ToList();
        vm.TrainerList = _dbContext.Users
            .OfType<Trainer>()
            .Select(t => new SelectListItem { Text = t.Name, Value = t.Id })
            .ToList();
        vm.RoomList = _dbContext.Rooms
            .Select(r => new SelectListItem { Text = r.RoomName, Value = r.RoomId.ToString() })
            .ToList();
        vm.BranchName = "Metro Branch";
        return View(vm);
      }

      var currentUser = await _userManager.GetUserAsync(User);
      if (currentUser == null)
      {
        return Forbid();
      }

      var session = new Session
      {
        SessionDateTime = vm.SessionDateTime,
        GymClassId = vm.GymClassId,
        TrainerId = vm.TrainerId,
        RoomId = vm.RoomId,
        Capacity = vm.Capacity,
        ReceptionistId = currentUser.Id
      };

      _dbContext.Sessions.Add(session);
      await _dbContext.SaveChangesAsync();
      return RedirectToAction("Dashboard");
    }

    [HttpGet]
    public IActionResult GetRoomCapacity(int roomId)
    {
      var capacity = _dbContext.Rooms.Where(r => r.RoomId == roomId)
          .Select(r => r.Capacity)
          .FirstOrDefault();

      return Json(new { capacity });
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
