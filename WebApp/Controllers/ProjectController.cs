using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.HelperModels;

namespace WebApp.Controllers;

public class ProjectController(UserManager<User> userManager, ApplicationContext db) : BaseController(userManager, db)
{
    public async Task<IActionResult> Browse()
    {
        var redirect = await IsNeedRedirect(CheckState.Login);
        if (redirect != null) return redirect;

        var user = await CurrentUser;
        var projects = await DbContext.Projects
            .Include(p => p.Members)
            .Include(p => p.Objectives)
            .Include(p => p.Roles)
            .Where(p => p.Members.Any(m => string.Equals(m.UserId, user!.Id)))
            .ToListAsync();

        return View(projects);
    }
    
    public async Task<IActionResult> Select()
    {
        var redirect = await IsNeedRedirect(CheckState.Login);
        if (redirect != null) return redirect;

        var user = await CurrentUser;
        var projects = await DbContext.Projects
            .Include(p => p.Members)
            .Include(p => p.Objectives)
            .Include(p => p.Roles)
            .Where(p => p.Members.Any(m => string.Equals(m.UserId, user!.Id)))
            .ToListAsync();
        
        return View(projects);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Select(Guid projectId)
    {
        var redirect = await IsNeedRedirect(CheckState.Login);
        if (redirect != null) return redirect;

        var user = await CurrentUser;
        var project = await DbContext.Projects.FindAsync(projectId);
        if (project == null)
            return NotFound();

        user!.SelectedProject = project;
        await UserManager.UpdateAsync(user);

        return RedirectToAction(nameof(Select));
    }
    
    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Project project)
    {
        var redirect = await IsNeedRedirect(CheckState.Login);
        if (redirect != null) return redirect;
        
        var user = await CurrentUser;
        try
        {
            if (ModelState.IsValid)
            {
                await SeedData.InitNewProject(DbContext, UserManager, user!, project);
                return RedirectToAction("Browse");
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "An error occurred while creating the project: " + ex.Message);
        }
    
        return View(project);
    }
}