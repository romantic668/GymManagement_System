using Microsoft.AspNetCore.Mvc;
using GymManagement.Models;
using GymManagement.Data;
using GymManagement.Helpers;
using GymManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace GymManagement.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public CustomerController(AppDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        private async Task<Customer> GetCurrentCustomerAsync()
        {
            var userId = _userManager.GetUserId(User); 
            var customer = await _dbContext.Customers
                .Include(c => c.Bookings)
                .Include(c => c.Payments)
                .FirstOrDefaultAsync(c => c.Id == userId);

            return customer!;
        }

        public async Task<IActionResult> Dashboard(string? category, string? trainer, string? status, int page = 1)
{
    var customer = await GetCurrentCustomerAsync();
    int pageSize = 10;
    var now = DateTime.Now;

    // 查询 Booking 数据
    var bookingsQuery = _dbContext.Bookings
        .Include(b => b.Session).ThenInclude(s => s.GymClass)
        .Include(b => b.Session).ThenInclude(s => s.Trainer)
        .Include(b => b.Session).ThenInclude(s => s.Room)
        .Where(b => b.CustomerId == customer.Id)
        .AsQueryable();

    // 筛选逻辑
    if (!string.IsNullOrEmpty(category))
        bookingsQuery = bookingsQuery.Where(b => b.Session.Category.ToString() == category);

    if (!string.IsNullOrEmpty(trainer))
        bookingsQuery = bookingsQuery.Where(b => b.Session.Trainer.Name == trainer);

    if (!string.IsNullOrEmpty(status) && Enum.TryParse<BookingStatus>(status, out var parsedStatus))
        bookingsQuery = bookingsQuery.Where(b => b.Status == parsedStatus);

    // 投影为 ViewModel
    var allBookingVMs = await bookingsQuery
        .OrderByDescending(b => b.Session.SessionDateTime)
        .Select(b => new BookingViewModel
        {
            BookingId = b.BookingId,
            ClassName = b.Session.GymClass.ClassName,
            SessionDate = b.Session.SessionDateTime,
            Status = b.Status.ToString(),
            TrainerName = b.Session.Trainer.Name,
            RoomName = b.Session.Room.RoomName,
            Category = b.Session.Category.ToString()
        })
        .ToListAsync();

    // 分组
    var upcomingAll = allBookingVMs.Where(b => b.SessionDate > now).ToList();
    var past = allBookingVMs.Where(b => b.SessionDate <= now).ToList();

    // 分页
    var totalPages = (int)Math.Ceiling(upcomingAll.Count / (double)pageSize);
    var upcomingPaged = upcomingAll
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToList();

    // 构建 ViewModel
    var dashboardVM = new CustomerDashboardViewModel
    {
        Name = customer.Name,
        MembershipType = customer.MembershipType.ToString(),
        MembershipStatus = customer.MembershipStatus.ToString(),
        SubscriptionDate = customer.SubscriptionDate,
        MembershipExpiry = customer.MembershipExpiry,
        ProfileImageName = customer.ProfileImageName ?? "default.png",
        Payments = customer.Payments.Select(p => new PaymentViewModel
        {
            Price = p.Price,
            PaymentMethod = p.PaymentMethod,
            PaymentDate = p.PaymentDate
        }).ToList(),
        UpcomingBookings = upcomingPaged,
        PastBookings = past,
        CurrentPage = page,
        TotalPages = totalPages
    };

    // 下拉选项
    ViewBag.CategoryOptions = Enum.GetNames(typeof(SessionCategory)).ToList();
    ViewBag.TrainerOptions = await _dbContext.Trainers.Select(t => t.Name).Distinct().ToListAsync();
    ViewBag.StatusOptions = Enum.GetNames(typeof(BookingStatus)).ToList();
    ViewBag.SelectedCategory = category;
    ViewBag.SelectedTrainer = trainer;
    ViewBag.SelectedStatus = status;

    return View(dashboardVM);
}


        // 2. Profile Info
        public async Task<IActionResult> Profile()
        {
            var customer = await GetCurrentCustomerAsync(); 
            return View(customer);  
        }

        [HttpPost]
        public async Task<IActionResult> Profile(Customer updatedCustomer)
        {
            var customer = await GetCurrentCustomerAsync();  

            if (!ModelState.IsValid)
                return View(updatedCustomer);  

            // 更新 Customer 信息
            customer.Name = updatedCustomer.Name;
            customer.PhoneNumber = updatedCustomer.PhoneNumber;

            _dbContext.Update(customer);  
            await _dbContext.SaveChangesAsync();  

            TempData["Success"] = "Profile updated!"; 
            return RedirectToAction("Dashboard"); 
        }

        // 3. Membership Status
        [HttpGet]
        public async Task<IActionResult> Membership()
        {
            var customer = await GetCurrentCustomerAsync();

            var vm = new MembershipViewModel
            {
                Name = customer.Name,
                MembershipType = customer.MembershipType.ToString(),
                MembershipStatus = customer.MembershipStatus.ToString(),
                ExpiryDate = customer.MembershipExpiry,
                RemainingDays = customer.MembershipExpiry.HasValue
                    ? (customer.MembershipExpiry.Value.Date - DateTime.UtcNow.Date).Days
                    : 0,
                Payments = customer.Payments.Select(p => new PaymentViewModel
                {
                    Price = p.Price,
                    PaymentMethod = p.PaymentMethod,
                    PaymentDate = p.PaymentDate
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> RenewMembership(string plan)
        {
            var customer = await GetCurrentCustomerAsync();

            customer.MembershipStatus = MembershipStatus.Active;
            customer.SubscriptionDate = DateTime.UtcNow;

            // 设置类型和时间
            MembershipType selectedType = MembershipType.Monthly;
            switch (plan)
            {
                case "Quarterly":
                    selectedType = MembershipType.Quarterly;
                    customer.MembershipExpiry = DateTime.UtcNow.AddMonths(3);
                    break;
                case "Yearly":
                    selectedType = MembershipType.Yearly;
                    customer.MembershipExpiry = DateTime.UtcNow.AddYears(1);
                    break;
                default:
                    customer.MembershipExpiry = DateTime.UtcNow.AddMonths(1);
                    break;
            }

            customer.MembershipType = selectedType;

            // ⬇ 添加 payment 记录
            var payment = new Payment
            {
                CustomerId = customer.Id,
                Price = PricingPlans.GetPrice(selectedType),
                PaymentMethod = "Online",
                PaymentDate = DateTime.UtcNow
            };

            _dbContext.Payments.Add(payment);
            _dbContext.Update(customer);
            await _dbContext.SaveChangesAsync();

            TempData["Toast"] = $"Membership renewed as {plan} plan!";
            return RedirectToAction("Membership");
        }




        // 4. My Bookings
        public async Task<IActionResult> MyBookings()
        {
            var customer = await GetCurrentCustomerAsync();  // 获取 Customer
            var bookings = await _dbContext.Bookings
                .Include(b => b.Session)
                .ThenInclude(s => s.Trainer)
                .Where(b => b.UserId == customer.Id)
                .ToListAsync();

            return View(bookings);  // 返回 MyBookings 视图
        }

        // 5. Book a Session
        public async Task<IActionResult> BookSession()
        {
            var sessions = await _dbContext.Sessions
                .Include(s => s.Trainer)
                .Where(s => s.SessionDateTime > DateTime.Now)
                .ToListAsync();

            return View(sessions);  // 返回 Session 选择视图
        }

        [HttpPost]
        public async Task<IActionResult> BookSession(int sessionId)
        {
            var customer = await GetCurrentCustomerAsync();  // 获取 Customer

            var booking = new Booking
            {
                SessionId = sessionId,
                UserId = customer.Id,  // 使用 customer.Id
                BookingDate = DateTime.Now
            };

            _dbContext.Bookings.Add(booking);  // 将预订加入数据库
            await _dbContext.SaveChangesAsync();  // 保存更改

            TempData["Success"] = "Session booked!";  // 预订成功信息
            return RedirectToAction("MyBookings");  // 重定向到 MyBookings 页面
        }

        // 6. Trainer List
        public async Task<IActionResult> TrainerList()
        {
            var trainers = await _dbContext.Trainers.ToListAsync();  // 获取所有教练信息
            return View(trainers);  // 返回 TrainerList 视图
        }

        // 7. Session Calendar
        public async Task<IActionResult> SessionCalendar()
        {
            var sessions = await _dbContext.Sessions
                .Include(s => s.Trainer)
                .OrderBy(s => s.SessionDateTime)
                .ToListAsync();

            return View(sessions);  // 返回 SessionCalendar 视图
        }

        [HttpPost]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            var booking = await _dbContext.Bookings
                .Include(b => b.Session)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null || booking.Status == BookingStatus.Canceled)
            {
                TempData["Toast"] = "Booking not found or already canceled.";
                return RedirectToAction("Dashboard");
            }

            if (booking.Session.SessionDateTime <= DateTime.Now)
            {
                TempData["Toast"] = "Cannot cancel past or ongoing sessions.";
                return RedirectToAction("Dashboard");
            }

            booking.Status = BookingStatus.Canceled;
            await _dbContext.SaveChangesAsync();

            TempData["Toast"] = "Booking canceled successfully.";
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public async Task<IActionResult> BookAgain(int bookingId)
        {
            var booking = await _dbContext.Bookings
                .Include(b => b.Session)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null || booking.Status != BookingStatus.Canceled)
            {
                TempData["Toast"] = "Only canceled bookings can be rebooked.";
                return RedirectToAction("Dashboard");
            }

            if (booking.Session.SessionDateTime <= DateTime.Now)
            {
                TempData["Toast"] = "Cannot rebook past sessions.";
                return RedirectToAction("Dashboard");
            }

            booking.Status = BookingStatus.Pending;
            booking.BookingDate = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            TempData["Toast"] = "Booking re-submitted successfully.";
            return RedirectToAction("Dashboard");
        }




    }
}
