using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.HelperModels;

namespace WebApp.Controllers;

public class StatusController(UserManager<User> userManager, ApplicationContext db) : BaseController(userManager, db)
{
    public async Task<IActionResult> Browse([FromRoute] Guid? projectId)
    {
        var redirect = await IsNeedRedirect();
        if (redirect != null) return redirect;

        var user = await CurrentUser;
        projectId ??= user!.SelectedProjectId;
        
        var statuses = await DbContext.Statuses
            .Include(s => s.Objectives)
            .Where(s => s.ProjectId == (Guid)projectId!)
            .ToListAsync();
        
        return View(statuses);
    }
    
    public async Task<IActionResult> Create([FromRoute] Guid? projectId)
    {
        var redirect = await IsNeedRedirect(CheckState.Project);
        if (redirect != null) return redirect;
        
        var user = await CurrentUser;
        projectId ??= user!.SelectedProjectId;
        var model = new Status()
        {
            ProjectId = (Guid)projectId!
        };
        return View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(Status model)
    {
        ModelState.Remove("Project");
        if (!ModelState.IsValid)
            return View(model);
        
        DbContext.Statuses.Add(model);
        await DbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Browse));
    }
    
    public async Task<IActionResult> Edit([FromRoute] Guid id)
    {
        var redirect = await IsNeedRedirect(CheckState.Project);
        if (redirect != null) return redirect;
        
        var status = await db.Statuses.FindAsync(id);
        if (status == null)
            return NotFound();
        
        return View(status);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Status status)
    {
        var redirect = await IsNeedRedirect(CheckState.Project);
        if (redirect != null) return redirect;
        
        ModelState.Remove("Project");
        if (!ModelState.IsValid)
            return View(status);
        
        db.Update(status);
        await db.SaveChangesAsync();
        
        return RedirectToAction(nameof(Browse));
    }

    [HttpPost]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var status = await db.Statuses.FindAsync(id);
        if (status == null)
            return RedirectToAction(nameof(Browse));
    
        db.Statuses.Remove(status);
        await db.SaveChangesAsync();
    
        return RedirectToAction(nameof(Browse));
    }
}