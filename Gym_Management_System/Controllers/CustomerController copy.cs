// CustomerController1.cs
using Microsoft.AspNetCore.Mvc;
using GymManagement.Models;
using GymManagement.Data;
using GymManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Diagnostics;

namespace GymManagement.Controllers
{
  [Authorize(Roles = "Customer")]
  public class CustomerController1 : Controller
  {
    private readonly AppDbContext _dbContext;
    private readonly UserManager<User> _userManager;
    public CustomerController1(AppDbContext dbContext, UserManager<User> userManager)
    {
      _dbContext = dbContext;
      _userManager = userManager;
    }

    // Search Customers
    [Authorize]
    public IActionResult Details(string searchQuery)
    {
      var customers = _dbContext.Customers.AsQueryable();
      if (!string.IsNullOrEmpty(searchQuery))
      {
        customers = customers.Where(c => c.Name.Contains(searchQuery));
      }
      return View(customers.ToList());
    }

    // Create Customer
    [Authorize]
    public IActionResult Create()
    {
      return View();
    }

    // Create Customer
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Customer customer)
    {
      _dbContext.Customers.Add(customer);
      _dbContext.SaveChanges();
      return RedirectToAction("Details");
    }

    // Delete Customer
    [Authorize]
    [HttpPost]
    public IActionResult Delete([FromBody] Dictionary<string, int> data)
    {
      if (!data.ContainsKey("id") || data["id"] == 0)
      {
        return Json(new { success = false, message = "No customer ID provided" });
      }

      int id = data["id"];
      var customer = _dbContext.Customers.Find(id);
      if (customer == null)
      {
        return Json(new { success = false, message = "Customer not found" });
      }

      _dbContext.Customers.Remove(customer);
      _dbContext.SaveChanges();
      return Json(new { success = true, message = "Customer deleted successfully" });
    }

    // Edit Customer
    [Authorize]
    public IActionResult Edit(int? id)
    {
      if (id == null)
      {
        return NotFound("Invalid Customer ID");
      }

      try
      {
        var customer = _dbContext.Customers.Find(id);
        if (customer == null)
        {
          return NotFound($"Customer with ID {id} not found");
        }

        return View(customer);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error fetching customer: {ex.Message}");
        return StatusCode(500, "Internal Server Error");
      }
    }

    // Edit Customer
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Customer customer)
    {
      var existingCustomer = _dbContext.Customers.FirstOrDefault(c => c.Id == customer.Id);
      if (existingCustomer == null)
      {
        return NotFound("Customer not found.");
      }

      try
      {
        _dbContext.Entry(existingCustomer).CurrentValues.SetValues(customer);
        _dbContext.SaveChanges();
        return RedirectToAction("Details");
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        ModelState.AddModelError("", "Database update failed.");
        return View(customer);
      }
    }

  }
}
