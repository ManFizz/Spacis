using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.SomeModels;

namespace WebApp.Controllers;

[Authorize]
public class StatusController(UserManager<User> userManager, ApplicationContext db) : Controller
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
        
        return View(await db.Statuses
            .Include(s => s.Objectives)
            .ToListAsync());
    }
    public IActionResult CreateStatus()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateStatus(Status status)
    {
        if (!ModelState.IsValid)
            return View(status);
        
        db.Statuses.Add(status);
        await db.SaveChangesAsync();
        return RedirectToAction("DisplayList");
    }
}