// AccountController.cs
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GymManagement.Data;

public class AccountController : Controller
{
  private readonly AppDbContext _dbContext;

  public AccountController(AppDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  // SignUp
  [HttpGet]
  public IActionResult SignUp()
  {
    return View();
  }

  // SignUp Request
  [HttpPost]
  public async Task<IActionResult> SignUp(string username, string email, string password)
  {
    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
      ViewBag.Error = "Username, email and password cannot be empty.";
      return View();
    }

    // If User exist
    if (_dbContext.Users.Any(u => u.Username == username || u.Email == email))
    {
      ViewBag.Error = "Username or email is already taken.";
      return View();
    }

    // Password Hash
    string passwordHash = HashPassword(password);
    Console.WriteLine($"[DEBUG] Hashed Password: {passwordHash}");

    // Create User
    var newUser = new User
    {
      Email = email,
      Username = username,
      PasswordHash = passwordHash,
      Role = "User"
    };



    _dbContext.Users.Add(newUser);
    await _dbContext.SaveChangesAsync();

    // After create, auto login
    var claims = new List<Claim>
        {
          new Claim(ClaimTypes.Email, email),
          new Claim(ClaimTypes.Name, username)
        };

    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var authProperties = new AuthenticationProperties { IsPersistent = true };

    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(claimsIdentity), authProperties);

    return RedirectToAction("Index", "Home");
  }

  // Login
  [HttpGet]
  public IActionResult Login()
  {
    return View();
  }

  // Login Request
  [HttpPost]
  public async Task<IActionResult> Login(string identifier, string password)
  {
    var user = _dbContext.Users.FirstOrDefault(u => u.Username == identifier || u.Email == identifier);

    if (user == null || !VerifyPassword(password, user.PasswordHash))
    {
      ViewBag.Error = "Invalid username/email or password";
      return View();
    }

    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role?? "User")
    };

    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var authProperties = new AuthenticationProperties { IsPersistent = true };

    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(claimsIdentity), authProperties);

    return RedirectToAction("Index", "Home");
  }

  // Logout
  [HttpPost]
  public async Task<IActionResult> Logout()
  {
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return RedirectToAction("Login");
  }

  // Hash password
  private string HashPassword(string password)
  {
    using var rng = RandomNumberGenerator.Create();
    byte[] salt = new byte[16];
    rng.GetBytes(salt);

    using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
    byte[] hash = pbkdf2.GetBytes(32);

    return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
  }

  // Validate password
  private bool VerifyPassword(string password, string storedHash)
  {
    var parts = storedHash.Split(':');
    if (parts.Length != 2) return false;

    byte[] salt = Convert.FromBase64String(parts[0]);
    byte[] storedHashBytes = Convert.FromBase64String(parts[1]);

    using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
    byte[] computedHash = pbkdf2.GetBytes(32);

    return computedHash.SequenceEqual(storedHashBytes);
  }
}
