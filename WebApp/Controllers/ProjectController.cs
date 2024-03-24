using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.SomeModels;

namespace WebApp.Controllers;

[Authorize]
public class ProjectController(UserManager<User> userManager, ApplicationContext db) : Controller
{
    public Task<IActionResult> DisplayList()
    {
        return Task.FromResult<IActionResult>(View(db.Projects
            .Include(p => p.Members)
            .Include(p => p.Objectives)
            .Include(p => p.Roles)));
    }
    public ActionResult CreateProject()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> CreateProject(Project project)
    {
        try
        {
            if (ModelState.IsValid)
            {
                db.Projects.Add(project);
                await db.SaveChangesAsync();
                return RedirectToAction("DisplayList");
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "An error occurred while creating the project: " + ex.Message);
        }
    
        return View(project);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SelectProject(Guid projectId)
    {
        var currentUser = await userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var project = await db.Projects.FindAsync(projectId);
        if (project == null)
        {
            return NotFound();
        }

        currentUser.SelectedProject = project;

        await userManager.UpdateAsync(currentUser);

        return RedirectToAction("DisplayList");
    }
}