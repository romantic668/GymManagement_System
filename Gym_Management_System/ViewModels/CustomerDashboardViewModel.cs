using GymManagement.Models;  
using GymManagement.ViewModels;
namespace GymManagement.ViewModels
{
  public class CustomerDashboardViewModel
  {
    public required string Name { get; set; }
    public required string MembershipType { get; set; }
    public required DateTime SubscriptionDate { get; set; }
    public required List<BookingViewModel> UpcomingBookings { get; set; }
    public required List<PaymentViewModel> Payments { get; set; }
    public required string MembershipStatus { get; set; }
    public required List<Booking> Bookings { get; set; } // 添加 Bookings 属性
  }
}
