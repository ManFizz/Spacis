using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.HelperModels;

namespace WebApp.Controllers;

public class ActionController(UserManager<User> userManager, ApplicationContext db) : BaseController(userManager, db)
{
    public async Task<IActionResult> Browse()
    {
        var redirect = await IsNeedRedirect();
        if (redirect != null) return redirect;

        var user = await CurrentUser;
        var actions = await DbContext.Actions
            .Include(a => a.Objective)
            .Include(a => a.Member)
            .Where(a => a.Objective.ProjectId == user!.SelectedProjectId)
            .ToListAsync();
        
        return View(actions);
    }
}
