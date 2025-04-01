using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using GymManagement.Models;
using GymManagement.ViewModels;

namespace GymManagement.Areas.Admin.Controllers
{
  [Authorize(Roles = "Admin")]
  [Area("Admin")]
  public class UserController : Controller
  {
    private readonly UserManager<User> userManager;
    private readonly RoleManager<IdentityRole> roleManager;

    public UserController(UserManager<User> userMgr, RoleManager<IdentityRole> roleMgr)
    {
      userManager = userMgr;
      roleManager = roleMgr;
    }

    // 🔹 Display user list + roles
    public async Task<IActionResult> Index()
    {
      var users = new List<User>();
      foreach (var user in userManager.Users)
      {
        user.RoleNames = await userManager.GetRolesAsync(user);
        users.Add(user);
      }

      var model = new UserViewModel
      {
        Users = users,
        Roles = roleManager.Roles
      };

      return View(model);
    }

    // 🔹 Add a user
    [HttpGet]
    public IActionResult Add() => View();

    [HttpPost]
    public async Task<IActionResult> Add(RegisterViewModel model)
    {
      if (ModelState.IsValid)
      {
        var user = new User
        {
          UserName = model.Username,
          Email = model.Email,
          Name = model.Name
        };

        var result = await userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
          return RedirectToAction("Index");

        foreach (var error in result.Errors)
          ModelState.AddModelError("", error.Description);
      }

      return View(model);
    }

    // 🔹 Delete user
    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
      var user = await userManager.FindByIdAsync(id);
      if (user != null)
      {
        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
          TempData["message"] = string.Join(" | ", result.Errors.Select(e => e.Description));
        }
      }

      return RedirectToAction("Index");
    }

    // 🔹 Add user to Admin role
    [HttpPost]
    public async Task<IActionResult> AddToAdmin(string id)
    {
      var user = await userManager.FindByIdAsync(id);
      var roleExists = await roleManager.RoleExistsAsync("Admin");

      if (!roleExists)
      {
        TempData["message"] = "Admin role does not exist. Please create it first.";
      }
      else if (user != null)
      {
        await userManager.AddToRoleAsync(user, "Admin");
      }

      return RedirectToAction("Index");
    }

    // 🔹 Remove from Admin role
    [HttpPost]
    public async Task<IActionResult> RemoveFromAdmin(string id)
    {
      var user = await userManager.FindByIdAsync(id);
      if (user != null)
      {
        await userManager.RemoveFromRoleAsync(user, "Admin");
      }

      return RedirectToAction("Index");
    }

    // 🔹 Delete Role
    [HttpPost]
    public async Task<IActionResult> DeleteRole(string id)
    {
      var role = await roleManager.FindByIdAsync(id);
      if (role != null)
      {
        await roleManager.DeleteAsync(role);
      }

      return RedirectToAction("Index");
    }

    // 🔹 Create Admin Role
    [HttpPost]
    public async Task<IActionResult> CreateAdminRole()
    {
      var result = await roleManager.CreateAsync(new IdentityRole("Admin"));
      if (!result.Succeeded)
      {
        TempData["message"] = string.Join(" | ", result.Errors.Select(e => e.Description));
      }

      return RedirectToAction("Index");
    }
  }
}
