using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers;

[Authorize]
public class StatusController(ApplicationContext db) : MainController(db)
{
    public Task<IActionResult> DisplayList()
    {
        ViewData["Title"] = "Statuses";
        return Task.FromResult<IActionResult>(View(db.Statuses
            .Include(s => s.Objectives)));
    }
    public IActionResult CreateStatus()
    {
        ViewData["Title"] = "Create status";
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> CreateStatus(Status status)
    {
        db.Statuses.Add(status);
        await db.SaveChangesAsync();
        return RedirectToAction("DisplayList");
    }
}