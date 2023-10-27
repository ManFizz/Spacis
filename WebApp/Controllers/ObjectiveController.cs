using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers;

public class ObjectiveController(ApplicationContext db) : MainController(db)
{
    
    public async Task<IActionResult> DisplayList()
    {
        return View(await db.Objectives
            .Include(o => o.Status)
            .Include(o => o.Labels)
            .ToListAsync());
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateObjective(Objective objective)
    {
        db.Objectives.Add(objective);
        await db.SaveChangesAsync();
        return RedirectToAction("DisplayList");
    }

    public IActionResult GetObjectiveInfo(string sGuid)
    {
        var guid = Guid.Parse(sGuid);
        var objective = db.Objectives
            .Include(objective => objective.Labels)
            .FirstOrDefault(o => o.Id == guid);
        
        if(objective == null)
            return Content("Error - data is invalid", "text/html");
        var formattedDate = $"{objective.DueDate:yyyy-MM-ddTHH:mm:ss}";
        var labels = string.Join("", objective.Labels.Select(label => $"<span class='badge bg-secondary'>{label.Name}</span>"));
        var taskInfoHtml = $"<h2>{objective.Title}</h2><br/><span class='formatted-date'>{formattedDate}</span>" +
                           $"<br/>{labels}<br/><span>{objective.Description}</span>";

        return Content(taskInfoHtml, "text/html");
    }
}