// Book.cs
public class Book
{
  public int BookId { get; set; }
  public required string Title { get; set; }

  // Associate to Author (a book has only one author)
  public int AuthorId { get; set; }
  public Author? Author { get; set; }

  // Associate to LibraryBranch (where a book exists)
  public int LibraryBranchId { get; set; }
  public LibraryBranch? LibraryBranch { get; set; }

  public int? CustomerId { get; set; }
  public Customer? Customer { get; set; }
  public List<BookInventory>? BookInventories { get; set; }
}
