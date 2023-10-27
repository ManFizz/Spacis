using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers;

public abstract class MainController(ApplicationContext db) : Controller
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}