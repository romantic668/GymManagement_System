// BookController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagement.Models;
using GymManagement.ViewModels;
using GymManagement.Data;
using System.Linq;

namespace GymManagement.Controllers
{
  public class BookController : Controller
  {
    private readonly AppDbContext _dbContext;
    public BookController(AppDbContext dbContext)
    {
      _dbContext = dbContext;
    }
    // Search Book
    public IActionResult Details(int? id, string searchQuery)
    {
      var books = _dbContext.Books.AsQueryable();

      if (!string.IsNullOrEmpty(searchQuery))
      {
        // Fuzzy query
        books = books.Where(b => b.Title.Contains(searchQuery));
      }
      // Calculate the total inventory per book
      // var bookTotalQuantities = _dbContext.BookInventories
      //     .GroupBy(b => b.BookId)
      //     .Select(group => new
      //     {
      //       BookId = group.Key,
      //       TotalQuantity = group.Sum(b => b.Quantity)
      //     })
      //     .ToDictionary(b => b.BookId, b => b.TotalQuantity);
      // Compute inventory per LibraryBranch
      // var branchInventory = _dbContext.BookInventories
      //     .Include(b => b.LibraryBranch)
      //     .Include(b => b.Book)
      //     .GroupBy(b => new { b.LibraryBranchId, b.BookId })
      //     .Select(group => new
      //     {
      //       LibraryBranchId = group.Key.LibraryBranchId,
      //       BranchName = group.First().LibraryBranch.Name,
      //       BookId = group.Key.BookId,
      //       BookTitle = group.First().Book.Title,
      //       Quantity = group.Sum(b => b.Quantity)
      //     })
      //     .ToList();

      // Get current book inventory
      // var bookInventory = new List<object>();
      // if (id.HasValue)
      // {
      //   bookInventory = _dbContext.BookInventories
      //       .Where(b => b.BookId == id.Value)
      //       .Include(b => b.LibraryBranch)
      //       .Select(b => (object)new
      //       {
      //         b.LibraryBranchId,
      //         BranchName = b.LibraryBranch.Name,
      //         b.Quantity
      //       })
      //       .ToList();
      // }

      // Passing data
      // ViewBag.BookTotalQuantities = bookTotalQuantities;
      // ViewBag.BranchInventory = branchInventory;
      // ViewBag.BookInventory = bookInventory;
      return View(books.ToList());
    }

    public IActionResult Create()
    {
      return View();
    }

    // POST: Create Book
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Book book)
    {
      if (!ModelState.IsValid)
      {
        return View(book);
      }

      _dbContext.Books.Add(book);
      _dbContext.SaveChanges();
      return RedirectToAction("Details");
    }

    // POST: Delete Book
    [HttpPost]
    public IActionResult Delete([FromBody] Dictionary<string, int> data)
    {
      if (!data.ContainsKey("id") || data["id"] == 0)
      {
        return Json(new { success = false, message = "No book ID provided" });
      }

      int id = data["id"];
      var book = _dbContext.Books.Find(id);
      if (book == null)
      {
        return Json(new { success = false, message = "Book not found" });
      }

      _dbContext.Books.Remove(book);
      _dbContext.SaveChanges();
      return Json(new { success = true, message = "Book deleted successfully" });
    }
    // GET: Edit Book
    public IActionResult Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var book = _dbContext.Books.FirstOrDefault(b => b.BookId == id);
      if (book == null)
      {
        return NotFound();
      }

      return View(book);
    }

    // POST: Edit Book
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Book book)
    {
      if (book == null || book.BookId == 0)
      {
        return BadRequest("Invalid book data.");
      }

      var existingBook = _dbContext.Books.FirstOrDefault(b => b.BookId == book.BookId);
      if (existingBook == null)
      {
        return NotFound("Book not found.");
      }

      try
      {
        _dbContext.Entry(existingBook).CurrentValues.SetValues(book);
        _dbContext.SaveChanges();
        return RedirectToAction("Details");
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        return View(book);
      }
    }
  }
}
