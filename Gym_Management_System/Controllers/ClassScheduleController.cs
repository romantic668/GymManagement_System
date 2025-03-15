// ClassScheduleController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagement.Data;
using GymManagement.Models;
using System.Security.Claims;

public class ClassScheduleController : Controller
{
  private readonly AppDbContext _dbContext;

  public ClassScheduleController(AppDbContext context)
  {
    _dbContext = context;
  }

  // 🔹 显示所有可用课程时间表
  public IActionResult Index()
  {
    var sessions = _dbContext.Sessions
        .Include(s => s.GymClass)
        .Include(s => s.Trainer)
        .Include(s => s.Room)
        .OrderBy(s => s.SessionDateTime)
        .ToList();

    return View(sessions);
  }

  // 🔹 课程详细信息
  public IActionResult SessionDetails(int sessionId)
  {
    var session = _dbContext.Sessions
        .Include(s => s.GymClass)
        .Include(s => s.Trainer)
        .Include(s => s.Room)
        .Include(s => s.Bookings)
            .ThenInclude(b => b.Customer)
        .FirstOrDefault(s => s.SessionId == sessionId);

    if (session == null)
    {
      return NotFound("Session not found.");
    }

    return View(session);
  }

  [Authorize(Roles = "Customer")]
  [HttpPost]
  public IActionResult BookSession(int sessionId)
  {
    int userId;
    var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!int.TryParse(userIdClaim, out userId))
    {
      return BadRequest("Invalid user identifier.");
    }
    var session = _dbContext.Sessions.FirstOrDefault(s => s.SessionId == sessionId);
    if (session == null)
    {
      return NotFound("Session not found.");
    }

    // 获取当前的会员
    var customer = _dbContext.Customers.FirstOrDefault(c => c.Id == userId);
    if (customer == null)
    {
      return NotFound("Customer not found.");
    }

    // 获取当前的接待员（如果适用）
    var receptionist = _dbContext.Receptionists.FirstOrDefault(r => r.Id == session.ReceptionistId);

    // 检查会员是否已经预约
    bool alreadyBooked = _dbContext.Bookings.Any(b => b.CustomerId == userId && b.SessionId == sessionId);
    if (alreadyBooked)
    {
      ViewBag.Error = "You have already booked this session.";
      return RedirectToAction("Index");
    }

    var booking = new Booking
    {
      BookingDate = DateTime.UtcNow,
      Status = BookingStatus.Pending,
      CustomerId = customer.Id,
      SessionId = session.SessionId,
      ReceptionistId = receptionist?.Id
    };

    _dbContext.Bookings.Add(booking);
    _dbContext.SaveChanges();

    return RedirectToAction("Index");
  }

  // 🔹 会员取消预约课程
  [Authorize(Roles = "Customer")]
  [HttpPost]
  public IActionResult CancelBooking(int bookingId)
  {
    int userId;
    var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!int.TryParse(userIdClaim, out userId))
    {
      return BadRequest("Invalid user identifier.");
    }

    var booking = _dbContext.Bookings.FirstOrDefault(b => b.BookingId == bookingId && b.CustomerId == userId);
    if (booking == null)
    {
      return NotFound("Booking not found.");
    }

    _dbContext.Bookings.Remove(booking);
    _dbContext.SaveChanges();

    return RedirectToAction("Index");
  }
}
