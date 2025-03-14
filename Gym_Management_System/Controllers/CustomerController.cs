// CustomerController.cs
using Microsoft.AspNetCore.Mvc;
using GymManagement.Models;
using GymManagement.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace GymManagement.Controllers
{
  [Authorize]
  public class CustomerController : Controller
  {
    private readonly AppDbContext _dbContext;

    public CustomerController(AppDbContext dbContext)
    {
      _dbContext = dbContext;
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
      customer.CustomerId = 0;
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
      if (customer.CustomerId == 0)
      {
        ModelState.AddModelError("", "Invalid customer data.");
        return View(customer);
      }

      var existingCustomer = _dbContext.Customers.FirstOrDefault(c => c.CustomerId == customer.CustomerId);
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
