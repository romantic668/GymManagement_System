using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymManagement.Data;
using GymManagement.Models;
using GymManagement.ViewModels;

using System.Security.Claims;

namespace GymManagement.Controllers
{
  [Authorize(Roles = "Receptionist")]
  public class ReceptionistController : Controller
  {
    private readonly AppDbContext _db;

    public ReceptionistController(AppDbContext db)
    {
      _db = db;
    }

    // 获取当前 Receptionist 实体
    private Receptionist? GetCurrentReceptionist()
    {
      string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      if (string.IsNullOrEmpty(userId)) return null;

      return _db.Receptionists.FirstOrDefault(r => r.Id == userId);
    }


    public IActionResult Dashboard()
    {
      var receptionist = GetCurrentReceptionist();
      if (receptionist == null)
        return NotFound("Receptionist not found.");

      var today = DateTime.Today;
      var branchId = receptionist.BranchId;

      var todaySessions = _db.Sessions
          .Include(s => s.Trainer)
          .Include(s => s.Room)
          .Include(s => s.GymClass)
          .Include(s => s.Bookings)
              .ThenInclude(b => b.Customer)
          .Where(s => s.SessionDateTime.Date == today && s.Room.BranchId == branchId)
          .ToList();

      var todayBookings = todaySessions.SelectMany(s => s.Bookings).ToList();

      var model = new ReceptionistDashboardViewModel
      {
        TodaySessions = todaySessions,
        TodayTotalBookings = todayBookings.Count,
        TodayCheckedIn = todayBookings.Count(b => b.Status == BookingStatus.CheckedIn),
        PendingBookings = todayBookings
              .OrderBy(b => b.Session.SessionDateTime)
              .ToList()
      };

      return View(model);
    }


    // 🔹 Search Member Info by Receptionist's Branch
    public IActionResult SearchMembersByBranch()
    {
      var receptionist = GetCurrentReceptionist();
      if (receptionist == null) return NotFound("Receptionist not found.");

      var members = _db.Customers
        .Where(c => c.GymBranchId == receptionist.BranchId)
        .ToList();

      return View(members);
    }



    // 🔹 Search Booking Info by Receptionist's Branch
    public IActionResult GetBookingsByBranch()
    {
      var receptionist = GetCurrentReceptionist();
      if (receptionist == null) return NotFound("Receptionist not found.");

      var bookings = _db.Bookings
        .Include(b => b.Customer)
        .Include(b => b.Session)
          .ThenInclude(s => s.GymClass)
        .Include(b => b.Session.Room)
        .Where(b => b.Session.Room.BranchId == receptionist.BranchId)
        .ToList();

      return View(bookings);
    }

    // 🔹 Check In Member
    [HttpPost]
    public IActionResult CheckInMember(int bookingId)
    {
      var booking = _db.Bookings
          .Include(b => b.Session)
          .Include(b => b.Customer)
          .FirstOrDefault(b => b.BookingId == bookingId);

      if (booking == null)
        return NotFound("Booking not found.");

      if (booking.Status == BookingStatus.Canceled)
        return BadRequest("Cannot check in a canceled booking.");

      if (booking.Status == BookingStatus.CheckedIn)
        return RedirectToAction("Dashboard"); // Already done

      // 打卡
      booking.Status = BookingStatus.CheckedIn;
      booking.CheckInTime = DateTime.Now;
      _db.SaveChanges();

      return RedirectToAction("Dashboard");
    }


    // 🔹 Get Room Info
    public IActionResult GetRoomInfoByBranch()
    {
      var receptionist = GetCurrentReceptionist();
      if (receptionist == null) return NotFound("Receptionist not found.");

      var rooms = _db.Rooms
        .Include(r => r.Sessions)
        .Where(r => r.BranchId == receptionist.BranchId)
        .ToList();

      return View(rooms);
    }

    // 🔹 Branch Session Calendar
    public IActionResult GetBranchSchedule()
    {
      var receptionist = GetCurrentReceptionist();
      if (receptionist == null) return NotFound("Receptionist not found.");

      var schedule = _db.Sessions
        .Include(s => s.GymClass)
        .Include(s => s.Trainer)
        .Include(s => s.Room)
        .Where(s => s.Room.BranchId == receptionist.BranchId)
        .OrderBy(s => s.SessionDateTime)
        .ToList();

      return View(schedule);
    }

    [HttpGet]
    public IActionResult ManageBookings()
    {
      var receptionist = GetCurrentReceptionist();
      if (receptionist == null) return NotFound();

      var bookings = _db.Bookings
          .Include(b => b.Customer)
          .Include(b => b.Session)
              .ThenInclude(s => s.GymClass)
          .Include(b => b.Session.Trainer)
          .Where(b => b.Session.Room.BranchId == receptionist.BranchId)
          .OrderByDescending(b => b.Session.SessionDateTime)
          .ToList();

      var sessions = _db.Sessions
          .Where(s => s.Room.BranchId == receptionist.BranchId)
          .OrderBy(s => s.SessionDateTime)
          .ToList();

      var customers = _db.Customers
          .Where(c => c.GymBranchId == receptionist.BranchId)
          .ToList();

      var vm = new ManageBookingsViewModel
      {
        AllBookings = bookings,
        CustomerList = customers.Select(c => new SelectListItem { Text = c.Name, Value = c.Id }).ToList(),
        SessionList = sessions.Select(s => new SelectListItem
        {
          Text = $"{s.GymClass.ClassName} - {s.SessionDateTime:MMM dd HH:mm}",
          Value = s.SessionId.ToString()
        }).ToList()
      };

      return View(vm);
    }

    [HttpPost]
    public IActionResult CreateBooking(CreateBookingInputModel input)
    {
      var booking = new Booking
      {
        CustomerId = input.CustomerId,
        UserId = input.CustomerId,
        BookingDate = DateTime.Now,
        SessionId = input.SessionId,
        Status = BookingStatus.Confirmed
      };

      _db.Bookings.Add(booking);
      _db.SaveChanges();
      return RedirectToAction("ManageBookings");
    }

    public IActionResult ConfirmBooking(int bookingId)
    {
      var booking = _db.Bookings.FirstOrDefault(b => b.BookingId == bookingId);
      if (booking == null) return NotFound();

      booking.Status = BookingStatus.Confirmed;
      _db.SaveChanges();

      return Ok();
    }

    [HttpPost]
    public IActionResult CancelBooking(int bookingId)
    {
      var booking = _db.Bookings.FirstOrDefault(b => b.BookingId == bookingId);
      if (booking == null) return NotFound();

      booking.Status = BookingStatus.Canceled;
      _db.SaveChanges();

      return Ok();
    }

    [HttpGet]
    public IActionResult TimetableBooking()
    {
      var receptionist = GetCurrentReceptionist();
      if (receptionist == null) return NotFound();

      var sessions = _db.Sessions
          .Include(s => s.GymClass)
          .Include(s => s.Room)
          .Include(s => s.Trainer)
          .Where(s => s.Room.BranchId == receptionist.BranchId)
          .OrderBy(s => s.SessionDateTime)
          .ToList();

      var customers = _db.Customers
          .Where(c => c.GymBranchId == receptionist.BranchId)
          .Select(c => new SelectListItem
          {
            Text = c.Name,
            Value = c.Id
          }).ToList();

      var vm = new TimetableBookingViewModel
      {
        Sessions = sessions,
        CustomerList = customers
      };

      return View(vm);
    }

    [HttpPost]
    public IActionResult BookFromTimetable(string customerId, int sessionId)
    {
      if (string.IsNullOrEmpty(customerId)) return BadRequest("Customer ID required.");

      var alreadyBooked = _db.Bookings.Any(b => b.CustomerId == customerId && b.SessionId == sessionId);
      if (alreadyBooked) return BadRequest("Customer already booked.");

      var booking = new Booking
      {
        CustomerId = customerId,
        SessionId = sessionId,
        Status = BookingStatus.Confirmed,
        BookingDate = DateTime.Now,
        UserId = customerId
      };

      _db.Bookings.Add(booking);
      _db.SaveChanges();

      return RedirectToAction("TimetableBooking");
    }


  }
}
