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

    // 🔹 Display Register page
    [HttpGet]
    public IActionResult Register() => View();

    // 🔹 Handle Register POST
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
      if (ModelState.IsValid)
      {
        // 🔸 Check if username already exists
        var existingUserByUsername = await _userManager.FindByNameAsync(model.Username);
        if (existingUserByUsername != null)
        {
          ModelState.AddModelError("Username", "This username is already taken.");
          return View(model);
        }

        // 🔸 Check if email already exists
        var existingUserByEmail = await _userManager.FindByEmailAsync(model.Email);
        if (existingUserByEmail != null)
        {
          ModelState.AddModelError("Email", "This email is already registered.");
          return View(model);
        }

        // 🔸 Create new user
        var user = new User
        {
          UserName = model.Username,
          Email = model.Email,
          Name = model.Name,
          JoinDate = DateTime.UtcNow
        };

        // 🔸 Save to Identity database
        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
          // 🔸 Assign default role
          await _userManager.AddToRoleAsync(user, "Customer");
          await _signInManager.SignInAsync(user, isPersistent: false);
          return RedirectToAction("Index", "Home");
        }

        // 🔸 Show identity creation errors
        foreach (var error in result.Errors)
          ModelState.AddModelError("", error.Description);
      }

      return View(model);
    }

    // 🔹 Display Login page
    [HttpGet]
    public IActionResult Login(string returnUrl = "")
    {
      var model = new LoginViewModel { ReturnUrl = returnUrl };
      return View(model);
    }

    // 🔹 Handle Login POST
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
      if (ModelState.IsValid)
      {
        // 🔸 Sign in with Identity username and password
        var result = await _signInManager.PasswordSignInAsync(
            model.Username, model.Password, isPersistent: model.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
          if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);
          else
            return RedirectToAction("Index", "Home");
        }
      }

      ModelState.AddModelError("", "Invalid username/password.");
      return View(model);
    }

    // 🔹 Logout and redirect to Home
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
      await _signInManager.SignOutAsync();
      return RedirectToAction("Index", "Home");
    }

    // 🔹 Initiate Google login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ExternalLogin(string provider, string? returnUrl = null)
    {
      var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnUrl });
      var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
      return Challenge(properties, provider);
    }

    // 🔹 Callback from Google
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
      if (info == null)
        return RedirectToAction(nameof(Login));

      // 🔸 Try to sign in with external login provider
      var result = await _signInManager.ExternalLoginSignInAsync(
          info.LoginProvider, info.ProviderKey, isPersistent: false);

      if (result.Succeeded)
        return Redirect(returnUrl);

      // 🔸 If user doesn't exist, create one using email
      var email = info.Principal.FindFirstValue(ClaimTypes.Email);
      if (email != null)
      {
        var user = new User
        {
          UserName = email,
          Email = email,
          Name = email,
          JoinDate = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(user);
        if (createResult.Succeeded)
        {
          await _userManager.AddLoginAsync(user, info);
          await _userManager.AddToRoleAsync(user, "Customer");
          await _signInManager.SignInAsync(user, isPersistent: false);
          return Redirect(returnUrl);
        }

        foreach (var error in createResult.Errors)
          ModelState.AddModelError("", error.Description);
      }

      return RedirectToAction(nameof(Login));
    }

    // 🔹 Access Denied
    public ViewResult AccessDenied() => View();

    // 🔹 Show Change Password form
    [HttpGet]
    public IActionResult ChangePassword()
    {
      var model = new ChangePasswordViewModel
      {
        Username = User.Identity?.Name ?? ""
      };
      return View(model);
    }

    // 🔹 Handle Change Password POST
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
      if (ModelState.IsValid)
      {
        var user = await _userManager.FindByNameAsync(model.Username);
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
