using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.HelperModels;

namespace WebApp.Controllers;

public class ObjectiveController(UserManager<User> userManager, ApplicationContext db) : BaseController(userManager, db)
{
    public async Task<IActionResult> Browse()
    {
        var redirect = await IsNeedRedirect();
        if (redirect != null) return redirect;

        var user = await CurrentUser;
        var objectives = await DbContext.Objectives
            .Include(o => o.Status)
            .Include(o => o.Labels)
            .Include(o => o.Project)
            .Include(o => o.Author)
            .Include(o => o.Members)
            .Where(o => o.ProjectId == user!.SelectedProjectId)
            .ToListAsync();
        
        return View(objectives);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(Objective objective)
    {
        DbContext.Objectives.Add(objective);
        await DbContext.SaveChangesAsync();
        return RedirectToAction("Browse");
    }

    public IActionResult GetInfo(string sGuid)
    {
        var guid = Guid.Parse(sGuid);
        var objective = DbContext.Objectives
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