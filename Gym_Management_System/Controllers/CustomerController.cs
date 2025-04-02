using Microsoft.AspNetCore.Mvc;
using GymManagement.Models;
using GymManagement.Data;
using GymManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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

        // 1. Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var customer = await GetCurrentCustomerAsync();  
            var bookings = await _dbContext.Bookings
                .Include(b => b.Session)
                .Where(b => b.UserId == customer.Id)  
                .ToListAsync();

            var dashboardVM = new CustomerDashboardViewModel
            {
                Name = customer.Name,
                MembershipStatus = customer.MembershipStatus,
                // Bookings = bookings,
                MembershipType = customer.MembershipType,
                SubscriptionDate = customer.SubscriptionDate,
                Payments = customer.Payments.Select(p => new PaymentViewModel {
                    Price = p.Price,
                    PaymentMethod = p.PaymentMethod,
                    PaymentDate = p.PaymentDate
                }).ToList(),
                Bookings = customer.Bookings.ToList(),
                UpcomingBookings = new List<BookingViewModel>(),
            };

            return View(dashboardVM);  
            // return View(customerDashboardVMs);  
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
        public async Task<IActionResult> Membership()
        {
            var customer = await GetCurrentCustomerAsync();  
            return View(customer);  
        }

        [HttpPost]
        public async Task<IActionResult> RenewMembership()
        {
            var customer = await GetCurrentCustomerAsync();  
            customer.MembershipStatus = "Active";
            customer.MembershipExpiry = DateTime.Now.AddMonths(1);  

            _dbContext.Update(customer);  // 更新数据库
            await _dbContext.SaveChangesAsync();  // 保存更改

            TempData["Success"] = "Membership renewed!";  // 续订成功信息
            return RedirectToAction("Membership");  // 重定向到 Membership 页面
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
    }
}
