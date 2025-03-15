public class CustomerDashboardViewModel
{
  public required string Name { get; set; }
  public required string MembershipType { get; set; }
  public required List<BookingViewModel> UpcomingBookings { get; set; }
  public required List<PaymentViewModel> Payments { get; set; }
}
