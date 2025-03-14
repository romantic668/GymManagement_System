// Customer.cs
public class Customer
{
  public int CustomerId { get; set; }
  public required string Name { get; set; }
  public required string Email { get; set; }
  public required string Phone { get; set; }
  public int? Age { get; set; }
  public string? Address { get; set; }
  public DateTime RegisteredDate { get; set; } = DateTime.UtcNow;

  // Relationship: one customer can borrow multiple books
  public List<Book>? BorrowedBooks { get; set; }
}
