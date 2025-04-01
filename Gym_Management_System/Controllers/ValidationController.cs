using Microsoft.AspNetCore.Mvc;
using System.Linq;
using GymManagement.Data; 
public class ValidationController : Controller
{
    private readonly AppDbContext _context;

    public ValidationController(AppDbContext context)
    {
        _context = context;
    }

    [AcceptVerbs("Get", "Post")]
    public JsonResult CheckEmail(string email)
    {
        var exists = _context.Users.Any(u => u.Email == email);
        if (exists)
        {
            return Json($"Email '{email}' is already registered.");
        }
        return Json(true);
    }

    [AcceptVerbs("Get", "Post")]
    public JsonResult CheckUsername(string username)
    {
        var exists = _context.Users.Any(u => u.UserName == username);
        if (exists)
        {
            return Json($"Username '{username}' is already taken.");
        }
        return Json(true);
    }

}
