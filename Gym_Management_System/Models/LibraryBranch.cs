// LibraryBranch.cs
public class LibraryBranch
{
  public int LibraryBranchId { get; set; }
  public required string Name { get; set; }
  public string? Address { get; set; }
  public string? Phone { get; set; }
  public string? Email { get; set; }
  public string? OpeningHours { get; set; }
  public List<Book>? Books { get; set; }
  public List<BookInventory>? BookInventories { get; set; }
}
