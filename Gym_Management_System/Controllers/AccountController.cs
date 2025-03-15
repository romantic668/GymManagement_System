using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using GymManagement.Data;
using GymManagement.Models;

public class AccountController : Controller
{
  private readonly AppDbContext _dbContext;

  public AccountController(AppDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  // 🔹 GET: 注册页面
  [HttpGet]
  public IActionResult Register()
  {
    return View();
  }

  // 🔹 POST: 处理注册请求
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Register(string name, string email, string password)
  {
    if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
      ViewBag.Error = "Name, email, and password cannot be empty.";
      return View();
    }

    if (_dbContext.Users.Any(u => u.Email == email))
    {
      ViewBag.Error = "Email is already taken.";
      return View();
    }

    string passwordHash = HashPassword(password);

    var newUser = new User
    {
      Name = name,
      Email = email,
      Password = passwordHash,
      Role = Role.Customer, // 默认注册为会员
      JoinDate = DateTime.UtcNow
    };

    _dbContext.Users.Add(newUser);
    await _dbContext.SaveChangesAsync();

    await SignInUser(newUser);

    return RedirectToDashboard(newUser.Role);
  }

  // 🔹 GET: 登录页面
  [HttpGet]
  public IActionResult Login()
  {
    return View();
  }

  // 🔹 POST: 处理登录请求
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Login(string email, string password)
  {
    var user = _dbContext.Users.AsNoTracking().FirstOrDefault(u => u.Email == email);

    if (user == null || !VerifyPassword(password, user.Password))
    {
      ViewBag.Error = "Invalid email or password.";
      return View();
    }

    await SignInUser(user);

    return RedirectToDashboard(user.Role);
  }

  // 🔹 POST: 退出登录
  [HttpPost]
  public async Task<IActionResult> Logout()
  {
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return RedirectToAction("Login");
  }

  // 🔹 GET: 查看 & 修改个人信息
  [HttpGet]
  public IActionResult EditProfile()
  {
    int userId;
    var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!int.TryParse(userIdClaim, out userId))
    {
      return BadRequest("Invalid user identifier.");
    }
    var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);

    if (user == null)
    {
      return NotFound("User not found.");
    }

    var model = new EditProfileViewModel
    {
      Name = user.Name,
      Email = user.Email ?? "No Email"
    };

    return View(model);
  }

  // 🔹 POST: 处理修改个人信息请求
  [HttpPost]
  [ValidateAntiForgeryToken]
  public IActionResult EditProfile(EditProfileViewModel model)
  {
    int userId;
    var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!int.TryParse(userIdClaim, out userId))
    {
      return BadRequest("Invalid user identifier.");
    }
    var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);

    if (user == null)
    {
      return NotFound("User not found.");
    }

    user.Name = model.Name;
    user.Email = model.Email;

    _dbContext.SaveChanges();
    return RedirectToAction("EditProfile");
  }

  // 🔹 处理用户登录逻辑
  private async Task SignInUser(User user)
  {
    var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email ?? "No Email Provided"),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var authProperties = new AuthenticationProperties { IsPersistent = true };

    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
  }

  // 🔹 根据角色跳转到对应的 Dashboard
  private IActionResult RedirectToDashboard(Role role)
  {
    return role switch
    {
      Role.Admin => RedirectToAction("Dashboard", "Admin"),
      Role.Trainer => RedirectToAction("Dashboard", "Trainer"),
      _ => RedirectToAction("Dashboard", "Customer")
    };
  }

  // 🔹 哈希密码 (PBKDF2)
  private string HashPassword(string password)
  {
    using var rng = RandomNumberGenerator.Create();
    byte[] salt = new byte[16];
    rng.GetBytes(salt);

    using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
    byte[] hash = pbkdf2.GetBytes(32);

    return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
  }

  // 🔹 验证密码
  private bool VerifyPassword(string password, string storedHash)
  {
    if (string.IsNullOrWhiteSpace(storedHash)) return false;

    var parts = storedHash.Split(':');
    if (parts.Length != 2) return false;

    byte[] salt = Convert.FromBase64String(parts[0]);
    byte[] storedHashBytes = Convert.FromBase64String(parts[1]);

    using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
    byte[] computedHash = pbkdf2.GetBytes(32);

    return computedHash.SequenceEqual(storedHashBytes);
  }
}
