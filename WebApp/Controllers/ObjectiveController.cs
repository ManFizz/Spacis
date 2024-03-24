using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.SomeModels;

namespace WebApp.Controllers;

[Authorize]
public class ObjectiveController(UserManager<User> userManager, ApplicationContext db) : Controller
{
    public async Task<IActionResult> DisplayList()
    {
        var currentUser = await userManager.GetUserAsync(User);
        if(currentUser == null)
            return RedirectToAction("Login", "Account");
        
        currentUser = await userManager.Users
            .Include(u => u.SelectedProject)
            .SingleOrDefaultAsync(u => u.Id == currentUser.Id);
        
        if(currentUser!.SelectedProject == null)
            return RedirectToAction("DisplayList", "Project");
        
        return View(await db.Objectives
            .Include(o => o.Status)
            .Include(o => o.Labels)
            .Include(o => o.Project)
            .Include(o => o.Author)
            .Include(o => o.Members)
            .Where(o => o.ProjectId == currentUser.SelectedProjectId)
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
        var formattedDate = $"{objective.DueDateTime:yyyy-MM-ddTHH:mm:ss}";
        var labels = string.Join("", objective.Labels.Select(label => $"<span class='badge bg-secondary'>{label.Title}</span>"));
        var taskInfoHtml = $"<h2>{objective.Title}</h2><br/><span class='formatted-date'>{formattedDate}</span>" +
                           $"<br/>{labels}<br/><span>{objective.Description}</span>";

        return Content(taskInfoHtml, "text/html");
    }
}