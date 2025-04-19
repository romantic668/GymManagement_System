public class PaymentViewModel
{
    public decimal Price { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
}
