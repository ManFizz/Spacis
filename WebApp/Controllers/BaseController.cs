using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.HelperModels;

namespace WebApp.Controllers;


public class BaseController : Controller
{
    protected enum CheckState
    {
        All = 0,
        Login = 1,
        Project = 2,
        Member = 3,
    }
    
    protected readonly UserManager<User> UserManager;
    protected readonly ApplicationContext DbContext;

    private readonly Lazy<Task<User?>> _currentUser;

    public BaseController(UserManager<User> userManager, ApplicationContext db)
    {
        UserManager = userManager;
        DbContext = db;
        _currentUser = new Lazy<Task<User?>>(async () => await InitializeCurrentUserAsync());
    }

    protected Task<User?> CurrentUser => _currentUser.Value;

    private async Task<User?> InitializeCurrentUserAsync()
    {
        var user = await UserManager.GetUserAsync(User);
        if (user != null)
        {
            return await UserManager.Users
                .Include(u => u.SelectedProject)
                .Include(u => u.SelectedMember)
                    .ThenInclude(m => m!.Role)
                        .ThenInclude(r => r.Permissions)
                .SingleOrDefaultAsync(u => u.Id == user.Id);
        }
        return null;
    }

    protected async Task<IActionResult?> IsNeedRedirect(CheckState checkState = CheckState.All)
    {
        var user = await CurrentUser;
        if (user == null)
            return RedirectToAction("Login", "Account");
        
        if (checkState >= CheckState.Project && user.SelectedProject == null)
            return RedirectToAction("Select", "Project");

        if (checkState >= CheckState.Member && user.SelectedMember == null)
            return RedirectToAction("Select", "Member");

        return null;
    }
}