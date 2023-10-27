using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers;

public class IndexController(ApplicationContext db) : MainController(db)
{
    public IActionResult Index()
    {
        return View();
    }
}