using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.HelperModels;
using WebApp.ViewModels.Objective;

namespace WebApp.Controllers;

public class ObjectiveController(UserManager<User> userManager, ApplicationContext db) : BaseController(userManager, db)
{
    public async Task<IActionResult> Browse()
    {
        var redirect = await IsNeedRedirect(CheckState.Member);
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
    
    public async Task<IActionResult> Create()
    {
        var redirect = await IsNeedRedirect(CheckState.Member);
        if (redirect != null) return redirect;
        
        var viewModel = new ObjectiveCreateViewModel
        {
            AvailableLabels = await DbContext.Labels.ToListAsync(),
            AvailableMembers = await DbContext.Members.ToListAsync(),
            DueDateTime = DateTime.Now.AddDays(1)
        };
        
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult>  Create(ObjectiveCreateViewModel model)
    {
        var redirect = await IsNeedRedirect(CheckState.Member);
        if (redirect != null) return redirect;
        
        if (!ModelState.IsValid) 
            return View(model);

        var user = await CurrentUser;
        var status = DbContext.Statuses
            .Where(s => s.Title == "В планах")
            .FirstOrDefault() ?? new Status
        {
            Project = user!.SelectedProject!,
            Title = "В планах",
            Color = Color.Primary,
        };
        var objective = new Objective
        {
            Title = model.Title,
            Description = model.Description,
            Author = user!.SelectedMember!,
            DueDateTime = model.DueDateTime,
            Labels = DbContext.Labels.Where(l => model.SelectedLabelIds.Contains(l.Id)).ToList(),
            Members = DbContext.Members.Where(m => model.SelectedMemberIds.Contains(m.Id)).ToList(),
            Project = user!.SelectedProject!,
            Priority = 0,
            Status = status,
        };
        
        DbContext.Add(objective);
        await DbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Browse));
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