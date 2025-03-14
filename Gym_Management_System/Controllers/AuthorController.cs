// AuthorController.cs
using Microsoft.AspNetCore.Mvc;
using GymManagement.Models;
using GymManagement.ViewModels;
using GymManagement.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Controllers
{
  [Authorize]
  public class AuthorController : Controller
  {

    private readonly AppDbContext _dbContext;

    public AuthorController(AppDbContext dbContext)
    {
      _dbContext = dbContext;
    }
    // Search Authors
    [Authorize]
    public IActionResult Details(string searchQuery)
    {
      var authors = _dbContext.Authors
               .Include(a => a.Books)
               .AsQueryable();

      if (!string.IsNullOrEmpty(searchQuery))
      {
        // Fuzzy query
        authors = authors.Where(a => a.Name.Contains(searchQuery));
      }

      return View(authors.ToList());
    }
    [Authorize]
    public IActionResult Create()
    {
      return View();
    }

    // POST: Create Author
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Author author)
    {
      author.AuthorId = 0;
      _dbContext.Authors.Add(author);
      _dbContext.SaveChanges();
      return RedirectToAction("Details");
    }

    // POST: Delete Author
    [Authorize]
    [HttpPost]
    public IActionResult Delete([FromBody] Dictionary<string, int> data)
    {
      if (!data.ContainsKey("id") || data["id"] == 0)
      {
        return Json(new { success = false, message = "No author ID provided" });
      }

      int id = data["id"];
      var author = _dbContext.Authors.Find(id);
      if (author == null)
      {
        return Json(new { success = false, message = "Author not found" });
      }
      // Remove books first, then authors
      _dbContext.Books.RemoveRange(author.Books);
      _dbContext.Authors.Remove(author);
      _dbContext.SaveChanges();
      return Json(new { success = true, message = "Author deleted successfully" });
    }
    public IActionResult Edit(int? id)
    {
      if (id == null)
      {
        return NotFound("Invalid Author ID");
      }

      try
      {
        var author = _dbContext.Authors
                .Include(a => a.Books)
                .FirstOrDefault(a => a.AuthorId == id);

        if (author == null)
        {
          return NotFound($"Author with ID {id} not found");
        }

        return View(author);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error fetching author: {ex.Message}");
        return StatusCode(500, "Internal Server Error");
      }
    }

    // 🔹 POST: Author/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Author author)
    {
      if (author.AuthorId == 0)
      {
        ModelState.AddModelError("", "Invalid author data.");
        return View(author);
      }
      var existingAuthor = _dbContext.Authors
                      .Include(a => a.Books)
                      .FirstOrDefault(a => a.AuthorId == author.AuthorId);
      if (existingAuthor == null)
      {
        return NotFound("Author not found.");
      }

      try
      {
        _dbContext.Entry(existingAuthor).CurrentValues.SetValues(author);
        _dbContext.SaveChanges();
        return RedirectToAction("Details");
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        ModelState.AddModelError("", "Database update failed.");
        return View(author);
      }
    }
  }
}
