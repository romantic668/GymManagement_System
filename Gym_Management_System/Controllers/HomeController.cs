// HomeController.cs
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GymManagement.Models;
using GymManagement.ViewModels;
namespace GymManagement.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult ClassSchedule()
    {
        return View();
    }

    public IActionResult MembershipOptions()
    {
        return View();
    }

}
