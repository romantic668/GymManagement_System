// BookInventoryController.cs
using Microsoft.AspNetCore.Mvc;
using GymManagement.Data;
using GymManagement.Models;
using System.Linq;

public class BookInventoryController : Controller
{
  private readonly AppDbContext _dbContext;

  public BookInventoryController(AppDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  // Show inventory for a particular library
  public IActionResult Index(int libraryBranchId)
  {
    var inventory = _dbContext.BookInventories
        .Where(b => b.LibraryBranchId == libraryBranchId)
        .Select(b => new
        {
          b.Book.Title,
          b.Quantity
        })
        .ToList();
    return Json(inventory);
  }

  // Borrow books (reduce inventory, record borrowings)
  [HttpPost]
  [ValidateAntiForgeryToken]
  public IActionResult BorrowBook(int bookId, int libraryBranchId)
  {
    if (bookId == 0 || libraryBranchId == 0)
    {
      return Json(new { success = false, message = "Invalid request parameters." });
    }

    int customerId = int.Parse(User.Claims.First(c => c.Type == "CustomerId").Value);

    var inventory = _dbContext.BookInventories
        .FirstOrDefault(b => b.BookId == bookId && b.LibraryBranchId == libraryBranchId);

    if (inventory == null || inventory.Quantity <= 0)
    {
      return Json(new { success = false, message = "Insufficient inventory." });
    }

    inventory.Quantity--;
    _dbContext.BorrowedBooks.Add(new BorrowedBook
    {
      BookId = bookId,
      CustomerId = customerId,
      LibraryBranchId = libraryBranchId,
      BorrowDate = DateTime.Now
    });

    _dbContext.SaveChanges();
    return Json(new { success = true, message = "Book borrowed successfully!" });
  }
  // ReturnBook
  [HttpPost]
  [ValidateAntiForgeryToken]
  public IActionResult ReturnBook(int bookId)
  {
    if (bookId == 0)
    {
      return Json(new { success = false, message = "Invalid request parameters." });
    }

    int customerId = int.Parse(User.Claims.First(c => c.Type == "CustomerId").Value);

    var borrowedBook = _dbContext.BorrowedBooks
        .FirstOrDefault(b => b.BookId == bookId && b.CustomerId == customerId && b.ReturnDate == null);

    if (borrowedBook == null)
    {
      return Json(new { success = false, message = "No active borrow record found." });
    }

    borrowedBook.ReturnDate = DateTime.Now;
    var inventory = _dbContext.BookInventories
        .FirstOrDefault(b => b.BookId == bookId && b.LibraryBranchId == borrowedBook.LibraryBranchId);

    if (inventory != null)
    {
      inventory.Quantity++;
    }

    _dbContext.SaveChanges();
    return Json(new { success = true, message = "Book returned successfully!" });
  }


}
