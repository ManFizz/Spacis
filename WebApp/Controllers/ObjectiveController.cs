using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers;

public class ObjectiveController(ApplicationContext db) : MainController(db)
{
    
    public async Task<IActionResult> ViewObjectives()
    {
        ViewData["Title"] = "Objectives";
        return View(await db.Objectives
            .Include(o => o.User)
            .Include(o => o.Group)
            .Include(o => o.Status)
            .Include(o => o.Labels)
            .ToListAsync());
    }
    public IActionResult CreateObjective()
    {
        ViewData["Title"] = "Create object";
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> CreateObjective(Objective objective)
    {
        db.Objectives.Add(objective);
        await db.SaveChangesAsync();
        return RedirectToAction("ViewObjectives");
    }
}