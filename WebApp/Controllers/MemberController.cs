using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.SomeModels;
using WebApp.ViewModels;

namespace WebApp.Controllers;

public class MemberController(UserManager<User> userManager, ApplicationContext db) : Controller
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

        var members = await db.Members
            .Include(m => m.Project)
            .Include(m => m.Role)
            .Include(m => m.Actions)
            .Include(m => m.Objectives)
            .Where(m => m.ProjectId == currentUser.SelectedProject!.Id)
            .ToListAsync();

        return View(members);
    }

    [HttpGet]
    public async Task<IActionResult> CreateMember()
    {
        var currentUser = await CheckUser();
        if (currentUser == null)
            return RedirectToAction("DisplayList", "Project");
        
        var model = new CreateMemberModel
        {
            Roles = await db.Roles
                .Where(r => r.ProjectId == currentUser.SelectedProjectId)
                .ToListAsync()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMember(CreateMemberModel memberModel)
    {
        if (!ModelState.IsValid) 
            return View(memberModel);

        var newMember = new Member
        {
            Name = memberModel.Name,
            RoleId = memberModel.RoleId,
            Info = memberModel.Info
        };

        db.Members.Add(newMember);
        await db.SaveChangesAsync();
        return RedirectToAction("DisplayList");
    }
}