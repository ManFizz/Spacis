using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.HelperModels;

namespace WebApp.Controllers;

public class LabelController(UserManager<User> userManager, ApplicationContext db) : BaseController(userManager, db)
{
    public async Task<IActionResult> Browse([FromRoute] Guid? projectId)
    {
        var redirect = await IsNeedRedirect(CheckState.Member);
        if (redirect != null) return redirect;
        
        var user = await CurrentUser;
        projectId ??= user!.SelectedProjectId;
        
        var labels = await DbContext.Labels
            .Include(l => l.Objectives)
            .Where(l => l.ProjectId == (Guid)projectId!)
            .ToListAsync();
        
        return View(labels);
    }
    
    public async Task<IActionResult> Create([FromRoute] Guid? projectId)
    {
        var redirect = await IsNeedRedirect(CheckState.Project);
        if (redirect != null) return redirect;
        
        var user = await CurrentUser;
        projectId ??= user!.SelectedProjectId;
        
        var model = new Label()
        {
            ProjectId = (Guid)projectId!
        };
        return View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(Label label)
    {
        ModelState.Remove("Project");
        if (!ModelState.IsValid) 
            return View(label);
        
        DbContext.Labels.Add(label);
        await DbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Browse));
    }
    
    public async Task<IActionResult> Edit([FromRoute] Guid? id)
    {
        var redirect = await IsNeedRedirect(CheckState.Project);
        if (redirect != null) return redirect;
        
        if (id == null)
            return NotFound();
        
        var label = await DbContext.Labels.FindAsync(id);
        if (label == null)
            return NotFound();
        
        return View(label);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Label label)
    {
        var redirect = await IsNeedRedirect(CheckState.Project);
        if (redirect != null) return redirect;
        
        ModelState.Remove("Project");
        if (!ModelState.IsValid)
            return View(label);
        
        DbContext.Update(label);
        await DbContext.SaveChangesAsync();
        
        return RedirectToAction(nameof(Browse));
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var label = await DbContext.Labels.FindAsync(id);
        if (label == null)
            return RedirectToAction(nameof(Browse));
    
        DbContext.Labels.Remove(label);
        await DbContext.SaveChangesAsync();
    
        return RedirectToAction(nameof(Browse));
    }
}