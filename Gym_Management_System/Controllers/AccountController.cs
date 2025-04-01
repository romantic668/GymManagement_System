using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GymManagement.Models;
using GymManagement.ViewModels;
using System.Security.Claims;

namespace GymManagement.Controllers
{
  public class AccountController : Controller
  {
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
      _userManager = userManager;
      _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
      if (ModelState.IsValid)
      {
        var existingUserByUsername = await _userManager.FindByNameAsync(model.Username);
        if (existingUserByUsername != null)
        {
          ModelState.AddModelError("Username", "This username is already taken.");
          return View(model);
        }

        var existingUserByEmail = await _userManager.FindByEmailAsync(model.Email);
        if (existingUserByEmail != null)
        {
          ModelState.AddModelError("Email", "This email is already registered.");
          return View(model);
        }

        var user = new User
        {
          UserName = model.Username,
          Email = model.Email,
          Name = model.Name,
          JoinDate = DateTime.UtcNow,
          DOB = model.DOB // ✅ 加上这一行

        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
          await _userManager.AddToRoleAsync(user, "Customer");
          user.RoleNames = await _userManager.GetRolesAsync(user);
          await _signInManager.SignInAsync(user, isPersistent: false);
          return RedirectToAction("Dashboard", "Customer");
        }

        foreach (var error in result.Errors)
        {
          if (error.Code.Contains("Password"))
            ModelState.AddModelError("Password", error.Description);
          else if (error.Code.Contains("Email"))
            ModelState.AddModelError("Email", error.Description);
          else if (error.Code.Contains("UserName"))
            ModelState.AddModelError("Username", error.Description);
          else
            ModelState.AddModelError(string.Empty, error.Description);
        }
      }

      return View(model);
    }

    [HttpGet]
    public IActionResult Login(string returnUrl = "")
    {
      var model = new LoginViewModel { ReturnUrl = returnUrl };
      return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
      if (!ModelState.IsValid)
        return View(model);

      var result = await _signInManager.PasswordSignInAsync(
          model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);

      if (result.Succeeded)
      {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user != null)
        {
          user.RoleNames = await _userManager.GetRolesAsync(user);

          if (await _userManager.IsInRoleAsync(user, "Admin"))
            return RedirectToAction("Dashboard", "Admin", new { area = "Admin" });

          if (await _userManager.IsInRoleAsync(user, "Trainer"))
            return RedirectToAction("Dashboard", "Trainer");

          if (await _userManager.IsInRoleAsync(user, "Receptionist"))
            return RedirectToAction("Dashboard", "Receptionist");

          if (await _userManager.IsInRoleAsync(user, "Customer"))
            return RedirectToAction("Dashboard", "Customer");
        }

        return RedirectToAction("Index", "Home");
      }

      ModelState.AddModelError(string.Empty, "Invalid username or password.");
      return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
      await _signInManager.SignOutAsync();
      return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ExternalLogin(string provider, string? returnUrl = null)
    {
      var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnUrl });
      var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
      return Challenge(properties, provider);
    }

    [HttpGet]
    public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
    {
      returnUrl ??= Url.Content("~/");

      if (remoteError != null)
      {
        ModelState.AddModelError("", $"Error from external provider: {remoteError}");
        return RedirectToAction(nameof(Login));
      }

      var info = await _signInManager.GetExternalLoginInfoAsync();
      if (info == null) return RedirectToAction(nameof(Login));

      var result = await _signInManager.ExternalLoginSignInAsync(
          info.LoginProvider, info.ProviderKey, isPersistent: false);

      if (result.Succeeded) return Redirect(returnUrl);

      var email = info.Principal.FindFirstValue(ClaimTypes.Email);
      var fullName = info.Principal.FindFirstValue(ClaimTypes.Name) ?? email ?? "Google User";

      if (email != null)
      {
        var user = new User
        {
          UserName = email,
          Email = email,
          Name = fullName,
          JoinDate = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(user);
        if (createResult.Succeeded)
        {
          await _userManager.AddLoginAsync(user, info);
          await _userManager.AddToRoleAsync(user, "Customer");
          user.RoleNames = await _userManager.GetRolesAsync(user);
          await _signInManager.SignInAsync(user, isPersistent: false);
          return RedirectToAction("Dashboard", "Customer");
        }

        foreach (var error in createResult.Errors)
          ModelState.AddModelError("", error.Description);
      }

      return RedirectToAction(nameof(Login));
    }

    public IActionResult AccessDenied() => View();

    [HttpGet]
    public IActionResult ChangePassword()
    {
      var model = new ChangePasswordViewModel
      {
        Username = User.Identity?.Name ?? ""
      };
      return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
      if (ModelState.IsValid)
      {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null)
        {
          ModelState.AddModelError("", "User not found.");
          return View(model);
        }

        var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        if (result.Succeeded)
        {
          TempData["message"] = "Password changed successfully.";
          return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
          ModelState.AddModelError("", error.Description);
      }

      return View(model);
    }
  }
}
