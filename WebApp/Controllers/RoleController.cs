using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.HelperModels;

namespace WebApp.Controllers;


public class RoleController(UserManager<User> userManager, ApplicationContext db) : BaseController(userManager, db)
{
    public async Task<IActionResult> Browse()
    {
        var redirect = await IsNeedRedirect();
        if (redirect != null) return redirect;

        var user = await CurrentUser;
        var roles = await DbContext.Roles
            .Include(r => r.Project)
            .Include(r => r.Permissions)
            .Where(r => r.ProjectId == user!.SelectedProjectId)
            .ToListAsync();
        
        return View(roles);
    }

    public async Task<IActionResult> Create()
    {
        var redirect = await IsNeedRedirect();
        if (redirect != null) return redirect;

        var user = await CurrentUser;
        var role = new Role()
        {
            ProjectId = user!.SelectedProject!.Id
        };
        return View(role);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Role role)
    {
        var projectExists = await DbContext.Projects.AnyAsync(p => p.Id == role.ProjectId);
        if (!projectExists)
        {
            ModelState.AddModelError("ProjectId", "Выбранный проект не существует.");
        }
        
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
        
        DbContext.Roles.Add(role);
        await DbContext.SaveChangesAsync();
        return RedirectToAction("Browse");
    }
}