using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.HelperModels;
using WebApp.ViewModels.Member;

namespace WebApp.Controllers;

public class MemberController(UserManager<User> userManager, ApplicationContext db) : BaseController(userManager, db)
{
    public async Task<IActionResult> Browse(Guid? projectId = null)
    {
        var redirect = await IsNeedRedirect(projectId == null ? CheckState.Project : CheckState.Login);
        if (redirect != null) return redirect;
        
        var user = await CurrentUser;
        projectId ??= user!.SelectedProject!.Id;
        var members = await DbContext.Members
            .Include(m => m.Project)
            .Include(m => m.Role)
            .Include(m => m.Actions)
            .Include(m => m.Objectives)
            .Where(m => m.ProjectId == projectId)
            .ToListAsync();
    
        var model = new BrowseMembersViewModel()
        {
            Members = members, 
            ProjectId = (Guid)projectId
        };
        return View(model);
    }
    
    public async Task<IActionResult> Select()
    {
        var redirect = await IsNeedRedirect(CheckState.Project);
        if (redirect != null) return redirect;

        var user = await CurrentUser;
        var members = await DbContext.Members
            .Include(m => m.Project)
            .Include(m => m.Role)
            .Include(m => m.Actions)
            .Include(m => m.Objectives)
            .Where(m => m.ProjectId == user!.SelectedProject!.Id)
            .ToListAsync();

        return View(members);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Select(Guid memberId)
    {
        var redirect = await IsNeedRedirect(CheckState.Project);
        if (redirect != null) return redirect;

        var member = await DbContext.Members.FindAsync(memberId);
        if (member == null)
            return NotFound();

        var user = await CurrentUser;
        user!.SelectedMember = member;
        await UserManager.UpdateAsync(user);

        return View();
    }

    public async Task<IActionResult> Create([FromRoute] Guid? projectId)
    {
        var redirect = await IsNeedRedirect(CheckState.Member);
        if (redirect != null) return redirect;

        var user = await CurrentUser;
        projectId ??= user!.SelectedProjectId;
        
        var model = new CreateMemberViewModel
        {
            Roles = await DbContext.Roles
                .Where(r => r.ProjectId == projectId)
                .ToListAsync(),
            Users = await DbContext.Users.ToListAsync(),
            ProjectId = (Guid)projectId!,
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateMemberViewModel memberModel)
    {
        var redirect = await IsNeedRedirect(CheckState.Member);
        if (redirect != null) return redirect;
        
        if (!ModelState.IsValid) 
            return View(memberModel);

        var newMember = new Member
        {
            Name = memberModel.Name,
            Info = memberModel.Info,
            RoleId = memberModel.RoleId,
            ProjectId = memberModel.ProjectId,
            UserId = memberModel.UserId,
        };

        DbContext.Members.Add(newMember);
        await DbContext.SaveChangesAsync();
        return RedirectToAction("Browse");
    }
    
    public async Task<IActionResult> Edit([FromRoute] Guid? id)
    {
        var redirect = await IsNeedRedirect(CheckState.Member);
        if (redirect != null) return redirect;

        if (id == null)
            return NotFound();
        
        var member = await DbContext.Members.FindAsync(id);
        if (member == null)
            return NotFound();
        
        var model = new EditMemberViewModel
        {
            Id = member.Id,
            ProjectId = member.ProjectId,
            Roles = await DbContext.Roles
                .Where(r => r.ProjectId == member.ProjectId)
                .ToListAsync(),
            Users = await DbContext.Users.ToListAsync(),
            RoleId = member.RoleId,
            Name = member.Name,
            Info = member.Info,
            UserId = member.UserId,
        };
        
        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditMemberViewModel memberModel)
    {
        var redirect = await IsNeedRedirect(CheckState.Member);
        if (redirect != null) return redirect;
        
        if (!ModelState.IsValid) 
            return View(memberModel);
        
        var member = await DbContext.Members.FirstOrDefaultAsync(m => m.Id == memberModel.Id);
        if (member == null)
            return NotFound();

        member.Name = memberModel.Name;
        member.Info = memberModel.Info;
        member.RoleId = memberModel.RoleId;
        member.ProjectId = memberModel.ProjectId;
        member.UserId = memberModel.UserId;
        
        DbContext.Update(member);
        await DbContext.SaveChangesAsync();
        
        return RedirectToAction(nameof(Browse));
    }

    [HttpPost]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var member = await DbContext.Members.FindAsync(id);
        if (member == null)
            return RedirectToAction(nameof(Browse));
    
        DbContext.Members.Remove(member);
        await DbContext.SaveChangesAsync();
    
        return RedirectToAction(nameof(Browse));
    }
}