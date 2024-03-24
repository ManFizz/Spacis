using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.SomeModels;

namespace WebApp.Controllers;


[Authorize(Roles=Constants.AdministratorsRole)]
public class RoleController(UserManager<User> userManager, ApplicationContext db) : Controller
{
    private async Task<User?> CheckUser()
    {
        var currentUser = await userManager.GetUserAsync(User);
        if (currentUser == null) 
            return null;
        
        currentUser = await userManager.Users
            .Include(u => u.SelectedProject)
            .SingleOrDefaultAsync(u => u.Id == currentUser.Id);
        if (currentUser!.SelectedProject == null)
            return null;

        return currentUser;
    }
    
    public async Task<IActionResult> DisplayList()
    {
        var currentUser = await CheckUser();
        if (currentUser == null)
            return RedirectToAction("DisplayList", "Project");
        
        return View(await db.Roles
                .Include(r => r.Project)
                .Include(r => r.Permissions)
                .Where(r => r.ProjectId == currentUser.SelectedProjectId)
                .ToListAsync());
    }

    public async Task<IActionResult> CreateRole()
    {
        var currentUser = await CheckUser();
        if (currentUser == null)
            return RedirectToAction("DisplayList", "Project");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRole(Role role)
    {
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values)
            {
                foreach (var errorError in error.Errors)
                {
                    Console.WriteLine(errorError.ErrorMessage);
                }

            }

            return View(role);
        }

        var currentUser = await CheckUser();
        if (currentUser == null)
            return RedirectToAction("DisplayList", "Project");
        
        role.ProjectId = currentUser.SelectedProject!.Id;
        currentUser.SelectedProject!.Roles.Add(role);
        await db.SaveChangesAsync();
        return RedirectToAction("DisplayList");
    }
}