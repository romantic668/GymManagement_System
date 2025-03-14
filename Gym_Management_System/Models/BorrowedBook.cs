// BorrowedBook.cs
public class BorrowedBook
{
  public int BorrowedBookId { get; set; }

  public int BookId { get; set; }
  public Book? Book { get; set; }

  public int CustomerId { get; set; }
  public Customer? Customer { get; set; }

  public int LibraryBranchId { get; set; }
  public LibraryBranch? LibraryBranch { get; set; }

  public DateTime BorrowDate { get; set; } = DateTime.Now;
  public DateTime? ReturnDate { get; set; }  // If NULL，didn't return.
}
