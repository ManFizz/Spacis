using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.SomeModels;

namespace WebApp.Controllers;

[Authorize]
public class LabelController(UserManager<User> userManager, ApplicationContext db) : Controller
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
        
        return View(await db.Labels
            .Include(l => l.Objectives)
            .ToListAsync());
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateLabel(Label label)
    {
        if (!ModelState.IsValid) 
            return View(label);
        
        db.Labels.Add(label);
        await db.SaveChangesAsync();
        return RedirectToAction("DisplayList");
    }
}