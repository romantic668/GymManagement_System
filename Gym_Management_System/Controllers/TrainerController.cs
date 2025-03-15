// TrainerController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagement.Data;
using GymManagement.Models;
using GymManagement.ViewModels;
using System.Security.Claims;

[Authorize(Roles = "Trainer")]  // 仅允许教练访问
public class TrainerController : Controller
{
  private readonly AppDbContext _dbContext;

  public TrainerController(AppDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  // 🔹 教练仪表盘
  public IActionResult Dashboard()
  {
    int trainerId;
    var trainerIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!int.TryParse(trainerIdClaim, out trainerId))
    {
      return BadRequest("Invalid Trainer ID.");
    }

    var trainer = _dbContext.Trainers
        .Include(t => t.GymClasses)
        .Include(t => t.Sessions)
        .FirstOrDefault(t => t.Id == trainerId);

    if (trainer == null)
    {
      return NotFound("Trainer not found.");
    }

    return View(trainer);
  }

  // 🔹 查看教练安排的课程
  public IActionResult ViewSessions()
  {
    int trainerId;
    var trainerIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!int.TryParse(trainerIdClaim, out trainerId))
    {
      return BadRequest("Invalid Trainer ID.");
    }
    var sessions = _dbContext.Sessions
        .Include(s => s.GymClass)
        .Include(s => s.Room)
        .Where(s => s.TrainerId == trainerId)
        .OrderBy(s => s.SessionDateTime)
        .ToList();

    return View(sessions);
  }

  // 🔹 课程详情
  public IActionResult SessionDetails(int sessionId)
  {
    int trainerId;
    var trainerIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!int.TryParse(trainerIdClaim, out trainerId))
    {
      return BadRequest("Invalid Trainer ID.");
    }
    var session = _dbContext.Sessions
        .Include(s => s.GymClass)
        .Include(s => s.Room)
        .Include(s => s.Bookings)
            .ThenInclude(b => b.Customer)
        .FirstOrDefault(s => s.SessionId == sessionId && s.TrainerId == trainerId);

    if (session == null)
    {
      return NotFound("Session not found.");
    }

    return View(session);
  }

  // 🔹 标记课程考勤
  [HttpPost]
  public IActionResult MarkAttendance(int bookingId)
  {
    var booking = _dbContext.Bookings.FirstOrDefault(b => b.BookingId == bookingId);

    if (booking == null)
    {
      return NotFound("Booking not found.");
    }

    booking.Status = BookingStatus.CheckedIn;
    booking.CheckInTime = DateTime.UtcNow;
    _dbContext.SaveChanges();

    return RedirectToAction("SessionDetails", new { sessionId = booking.SessionId });
  }

  // 🔹 修改个人信息
  public IActionResult EditProfile()
  {
    int trainerId;
    var trainerIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!int.TryParse(trainerIdClaim, out trainerId))
    {
      return BadRequest("Invalid Trainer ID.");
    }
    var trainer = _dbContext.Trainers.FirstOrDefault(t => t.Id == trainerId);

    if (trainer == null)
    {
      return NotFound("Trainer not found.");
    }

    var model = new EditTrainerProfileViewModel
    {
      TrainerId = trainer.Id,
      Name = trainer.Name ?? "Unknown",
      Email = trainer.Email ?? "No Email Provided",
      Specialization = trainer.Specialization,
      ExperienceStarted = trainer.ExperienceStarted
    };

    return View(model);
  }

  // 🔹 处理修改个人信息请求
  [HttpPost]
  public IActionResult EditProfile(EditTrainerProfileViewModel model)
  {
    int trainerId;
    var trainerIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!int.TryParse(trainerIdClaim, out trainerId))
    {
      return BadRequest("Invalid Trainer ID.");
    }
    var trainer = _dbContext.Trainers.FirstOrDefault(t => t.Id == trainerId);

    if (trainer == null)
    {
      return NotFound("Trainer not found.");
    }

    trainer.Name = model.Name;
    trainer.Email = model.Email;
    trainer.Specialization = model.Specialization ?? "General";


    _dbContext.SaveChanges();
    return RedirectToAction("Dashboard");
  }
}
