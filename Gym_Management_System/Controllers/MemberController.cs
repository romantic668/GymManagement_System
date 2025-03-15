// MemberController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagement.Data;
using GymManagement.Models;
using System.Security.Claims;

[Authorize(Roles = "Customer")]  // 仅允许会员访问
public class MemberController : Controller
{
  private readonly AppDbContext _dbContext;

  public MemberController(AppDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  // 🔹 会员仪表盘
  public IActionResult Dashboard()
  {
    int userId;
    var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!int.TryParse(userIdClaim, out userId))
    {
      return BadRequest("Invalid user identifier.");
    }

    var customer = _dbContext.Customers
        .Include(c => c.Bookings)
        .ThenInclude(b => b.Session)
        .ThenInclude(s => s.GymClass)
        .Include(c => c.Payments)
        .FirstOrDefault(c => c.Id == userId);

    if (customer?.Name == null)
    {
      return NotFound("Customer not found.");
    }

    return View(customer);
  }

  // 🔹 预约健身课程
  [HttpPost]
  public IActionResult BookSession(int sessionId)
  {
    int userId;
    var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!int.TryParse(userIdClaim, out userId))
    {
      return BadRequest("Invalid user identifier.");
    }
    var session = _dbContext.Sessions.Find(sessionId);
    if (session == null)
    {
      return NotFound("Session not found.");
    }

    // 检查是否已经预约
    bool alreadyBooked = _dbContext.Bookings.Any(b => b.CustomerId == userId && b.SessionId == sessionId);
    if (alreadyBooked)
    {
      ViewBag.Error = "You have already booked this session.";
      return RedirectToAction("Dashboard");
    }

    var booking = new Booking
    {
      CustomerId = userId,
      SessionId = sessionId,
      BookingDate = DateTime.UtcNow,
      Status = BookingStatus.Pending
    };

    _dbContext.Bookings.Add(booking);
    _dbContext.SaveChanges();

    return RedirectToAction("Dashboard");
  }

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

    return RedirectToAction("Dashboard");
  }


  // 🔹 会员状态
  public IActionResult MembershipStatus()
  {
    int userId;
    var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!int.TryParse(userIdClaim, out userId))
    {
      return BadRequest("Invalid user identifier.");
    }
    var customer = _dbContext.Customers.FirstOrDefault(c => c.Id == userId);

    if (customer == null)
    {
      return NotFound("Customer not found.");
    }

    return View(customer);
  }

  // 🔹 健身记录
  public IActionResult WorkoutHistory()
  {
    int userId;
    var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!int.TryParse(userIdClaim, out userId))
    {
      return BadRequest("Invalid user identifier.");
    }

    var workoutHistory = _dbContext.Bookings
        .Include(b => b.Session)
            .ThenInclude(s => s.GymClass)
        .Where(b => b.CustomerId == userId && b.Status == BookingStatus.CheckedIn)
        .OrderByDescending(b => b.BookingDate)
        .ToList();


    return View(workoutHistory);
  }

  // 🔹 查看个人信息
  public IActionResult Profile()
  {
    int userId;
    var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!int.TryParse(userIdClaim, out userId))
    {
      return BadRequest("Invalid user identifier.");
    }
    var customer = _dbContext.Customers.FirstOrDefault(c => c.Id == userId);

    if (customer == null)
    {
      return NotFound("Customer not found.");
    }

    return View(customer);
  }

  // 🔹 修改个人信息
  [HttpPost]
  public IActionResult EditProfile(Customer updatedCustomer)
  {
    int userId;
    var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!int.TryParse(userIdClaim, out userId))
    {
      return BadRequest("Invalid user identifier.");
    }
    var existingCustomer = _dbContext.Customers.FirstOrDefault(c => c.Id == userId);

    if (existingCustomer == null)
    {
      return NotFound("Customer not found.");
    }

    existingCustomer.Name = updatedCustomer.Name;
    existingCustomer.Email = updatedCustomer.Email;
    existingCustomer.MembershipType = updatedCustomer.MembershipType;

    _dbContext.SaveChanges();
    return RedirectToAction("Profile");
  }
}
