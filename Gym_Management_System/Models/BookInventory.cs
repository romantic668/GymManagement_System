// BookInventory.cs
public class BookInventory
{
  public int BookInventoryId { get; set; }

  public int BookId { get; set; }
  public Book? Book { get; set; }

  public int LibraryBranchId { get; set; }
  public LibraryBranch? LibraryBranch { get; set; }

  public int Quantity { get; set; }
}
