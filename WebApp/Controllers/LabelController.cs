using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers;

public class LabelController(ApplicationContext db) : MainController(db)
{
    public Task<IActionResult> DisplayList()
    {
        return Task.FromResult<IActionResult>(View(db.Labels
            .Include(l => l.User)
            .Include(l => l.Objectives)));
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateLabel(Label label)
    {
        db.Labels.Add(label);
        await db.SaveChangesAsync();
        return RedirectToAction("DisplayList");
    }
}