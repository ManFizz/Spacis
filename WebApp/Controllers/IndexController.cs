using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.SomeModels;

namespace WebApp.Controllers;

public class IndexController(ApplicationContext db) : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}