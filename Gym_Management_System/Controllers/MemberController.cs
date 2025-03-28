using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagement.Data;
using GymManagement.Models;
using System.Security.Claims;

[Authorize(Roles = "Customer")] // Only allow access to Customers
public class MemberController : Controller
{
  private readonly AppDbContext _dbContext;

  public MemberController(AppDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  // 🔹 Member Dashboard
  public IActionResult Dashboard()
  {
    string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(userId))
    {
      return BadRequest("Invalid user identifier.");
    }

    var customer = _dbContext.Customers
        .Include(c => c.Bookings)
            .ThenInclude(b => b.Session)
                .ThenInclude(s => s.GymClass)
        .Include(c => c.Payments)
        .FirstOrDefault(c => c.Id == userId);

    if (customer == null || customer.Name == null)
    {
      return NotFound("Customer not found.");
    }

    return View(customer);
  }

  // 🔹 Book a Gym Session
  [HttpPost]
  public IActionResult BookSession(int sessionId)
  {
    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(userId))
    {
      return BadRequest("Invalid user identifier.");
    }

    var session = _dbContext.Sessions.Find(sessionId);
    if (session == null)
    {
      return NotFound("Session not found.");
    }

    bool alreadyBooked = _dbContext.Bookings.Any(b => b.CustomerId == userId && b.SessionId == sessionId);
    if (alreadyBooked)
    {
      TempData["Error"] = "You have already booked this session.";
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

  // 🔹 Cancel a Booking
  [HttpPost]
  public IActionResult CancelBooking(int bookingId)
  {
    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(userId))
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

  // 🔹 Membership Status
  public IActionResult MembershipStatus()
  {
    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(userId))
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

  // 🔹 Workout History
  public IActionResult WorkoutHistory()
  {
    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(userId))
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

  // 🔹 View Profile
  public IActionResult Profile()
  {
    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(userId))
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

  // 🔹 Edit Profile
  [HttpPost]
  public IActionResult EditProfile(Customer updatedCustomer)
  {
    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(userId))
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
