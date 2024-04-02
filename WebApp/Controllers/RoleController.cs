using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.HelperModels;
using WebApp.ViewModels.Role;

namespace WebApp.Controllers;


public class RoleController(UserManager<User> userManager, ApplicationContext db) : BaseController(userManager, db)
{
    public async Task<IActionResult> Browse()
    {
        var redirect = await IsNeedRedirect(CheckState.Member);
        if (redirect != null) return redirect;

        var user = await CurrentUser;
        var roles = await DbContext.Roles
            .Include(r => r.Project)
            .Include(r => r.Permissions)
            .Where(r => r.ProjectId == user!.SelectedProjectId)
            .ToListAsync();
        
        return View(roles);
    }

    public async Task<IActionResult> Create([FromRoute] Guid? projectId)
    {
        var redirect = await IsNeedRedirect(CheckState.Member);
        if (redirect != null) return redirect;

        var user = await CurrentUser;
        projectId ??= user!.SelectedProjectId;
        var role = new Role()
        {
            ProjectId = (Guid)projectId!
        };
        return View(role);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Role role)
    {
        var projectExists = await DbContext.Projects.AnyAsync(p => p.Id == role.ProjectId);
        if (!projectExists)
            ModelState.AddModelError("ProjectId", "Выбранный проект не существует.");

        ModelState.Remove("Project");
        if (!ModelState.IsValid)
            return View(role);
        
        DbContext.Roles.Add(role);
        await DbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Browse));
    }
    
    public async Task<IActionResult> Edit([FromRoute] Guid? id)
    {
        if (id == null)
            return NotFound();
        
        var redirect = await IsNeedRedirect(CheckState.Project);
        if (redirect != null) return redirect;
        
        
        var role = await DbContext.Roles
            .Include(r => r.Permissions)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
        if (role == null)
            return NotFound();
        
        var allPermissions = await DbContext.Permissions.ToListAsync();
        var viewModel = new EditRoleViewModel
        {
            Id = role.Id,
            Title = role.Title,
            Info = role.Info,
            ProjectId = role.ProjectId,
            Permissions = allPermissions.Select(p => new AssignedPermissionData
            {
                PermissionId = p.Id,
                Title = p.Title,
                Assigned = role.Permissions.Any(rp => rp.Id == p.Id)
            }).ToList()
        };
        
        return View(viewModel);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Info,ProjectId")] EditRoleViewModel viewModel, Guid[] selectedPermissions)
    {
        var redirect = await IsNeedRedirect(CheckState.Project);
        if (redirect != null) return redirect;
        
        if (id != viewModel.Id)
            return NotFound();
        
        ModelState.Remove("Project");

        if (!ModelState.IsValid)
            return View(viewModel);
        
        var roleToUpdate = await DbContext.Roles
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (!await TryUpdateModelAsync(roleToUpdate!, "",
                r => r.Title, r => r.Info, r => r.ProjectId)) 
            return View(viewModel);
        
        UpdateRolePermissions(selectedPermissions, roleToUpdate!);
        await DbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Browse));
    }
    
    private void UpdateRolePermissions(IEnumerable<Guid> selectedPermissions, Role roleToUpdate)
    {
        var selectedPermissionsHs = new HashSet<Guid>(selectedPermissions);
        var rolePermissions = new HashSet<Guid>(roleToUpdate.Permissions.Select(p => p.Id));
        foreach (var permission in DbContext.Permissions)
        {
            if (selectedPermissionsHs.Contains(permission.Id))
            {
                if (!rolePermissions.Contains(permission.Id)) 
                    roleToUpdate.Permissions.Add(permission);
            }
            else
            {
                if (rolePermissions.Contains(permission.Id)) 
                    roleToUpdate.Permissions.Remove(permission);
            }
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var role = await DbContext.Roles.FindAsync(id);
        if (role == null)
            return RedirectToAction(nameof(Browse));
    
        DbContext.Roles.Remove(role);
        await DbContext.SaveChangesAsync();
    
        return RedirectToAction(nameof(Browse));
    }
}