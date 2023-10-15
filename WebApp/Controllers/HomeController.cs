using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers;

public class HomeController(ApplicationContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        return View(await db.Objectives
            .Include(o => o.User)
            .Include(o => o.Group)
            .Include(o => o.Status)
            .Include(o => o.Labels)
            .ToListAsync());
    }
    public IActionResult CreateObjective()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> CreateObjective(Objective objective)
    {
        db.Objectives.Add(objective);
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
    }
    
    public IActionResult CreateUser()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> CreateUser(User user)
    {
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return RedirectToAction("ViewUsers");
    }
    
    public Task<IActionResult> ViewUsers()
    {
        return Task.FromResult<IActionResult>(View(db.Users));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}