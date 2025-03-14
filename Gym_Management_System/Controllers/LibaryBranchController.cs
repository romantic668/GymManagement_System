using Microsoft.AspNetCore.Mvc;
using GymManagement.Models;
using GymManagement.Data;
using System.Linq;
using System.Collections.Generic;

namespace GymManagement.Controllers
{
  public class LibraryBranchController : Controller
  {
    private readonly AppDbContext _dbContext;

    public LibraryBranchController(AppDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    // Search LibraryBranch
    public IActionResult Details(string searchQuery)
    {
      var branches = _dbContext.LibraryBranches.AsQueryable();

      if (!string.IsNullOrEmpty(searchQuery))
      {
        // Fuzzy
        branches = branches.Where(b => b.Name.Contains(searchQuery));
      }

      return View(branches.ToList());
    }

    // Create LibraryBranch
    public IActionResult Create()
    {
      return View();
    }

    // Create LibraryBranch
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(LibraryBranch branch)
    {
      if (!ModelState.IsValid)
      {
        return View(branch);
      }

      _dbContext.LibraryBranches.Add(branch);
      _dbContext.SaveChanges();
      return RedirectToAction("Details");
    }

    // Delete LibraryBranch
    [HttpPost]
    public IActionResult Delete([FromBody] Dictionary<string, int> data)
    {
      if (!data.ContainsKey("id") || data["id"] == 0)
      {
        return Json(new { success = false, message = "No branch ID provided" });
      }

      int id = data["id"];
      var branch = _dbContext.LibraryBranches.Find(id);
      if (branch == null)
      {
        return Json(new { success = false, message = "LibraryBranch not found" });
      }

      _dbContext.LibraryBranches.Remove(branch);
      _dbContext.SaveChanges();
      return Json(new { success = true, message = "LibraryBranch deleted successfully" });
    }

    // Edit LibraryBranch
    public IActionResult Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var branch = _dbContext.LibraryBranches.FirstOrDefault(b => b.LibraryBranchId == id);
      if (branch == null)
      {
        return NotFound();
      }

      return View(branch);
    }

    // Edit LibraryBranch
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(LibraryBranch branch)
    {
      if (branch == null || branch.LibraryBranchId == 0)
      {
        return BadRequest("Invalid branch data.");
      }

      var existingBranch = _dbContext.LibraryBranches.FirstOrDefault(b => b.LibraryBranchId == branch.LibraryBranchId);
      if (existingBranch == null)
      {
        return NotFound("LibraryBranch not found.");
      }

      try
      {
        _dbContext.Entry(existingBranch).CurrentValues.SetValues(branch);
        _dbContext.SaveChanges();
        return RedirectToAction("Details");
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        return View(branch);
      }
    }
  }
}
