using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Models;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw)
    {
        await using var context = new ApplicationContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationContext>>());
        
        User adminUser = new()
        {
            UserName = Constants.AdministratorsRole,
            EmailConfirmed = true,
            Email = Constants.AdministratorsEmail,
        };
        var adminId = await EnsureUser(serviceProvider, testUserPw, adminUser);
        await EnsureRole(serviceProvider, adminId, Constants.AdministratorsRole);
        
        await EnsureRole(serviceProvider, adminId, Constants.UsersRole);

        User moderatorUser = new()
        {
            UserName = Constants.ModeratorsRole,
            EmailConfirmed = true,
            Email = Constants.ModeratorsEmail,
        };
        var moderatorId = await EnsureUser(serviceProvider, testUserPw, moderatorUser);
        await EnsureRole(serviceProvider, moderatorId, Constants.ModeratorsRole);

        await EnsureRole(serviceProvider, moderatorId, Constants.UsersRole);

        await InitData(context, adminId);

    }

    private static async Task InitData(ApplicationContext db, string adminId)
    {
        var study = new Group()
        {
            Id = Guid.NewGuid(),
            UserId = adminId,
            Name = "Study",
            Priority = 7
        };
        {
            var result = await db.Groups.FirstOrDefaultAsync(g => g.Name == study.Name);
            if (result == null)
                db.Groups.Add(study);
            else study = result;
        }

        var inProgress = new Status()
        {
            Id = Guid.NewGuid(),
            Name = "In progress"
        };
        {
            var result = await db.Statuses.FirstOrDefaultAsync(s => s.Name == inProgress.Name);
            if (result == null)
                db.Statuses.Add(inProgress);
            else inProgress = result;
        }

        var spacis = new Objective()
        {
            Id = Guid.NewGuid(),
            UserId = adminId,
            GroupId = study.Id,
            Title = "Spacis project",
            Description = "Create website on c# using ASP.NET and Entity Framework",
            DueDate = new DateTime(2024, 1, 14),
            StatusId = inProgress.Id,
            Priority = 99,
        };
        {
            var result = await db.Objectives.Include(objective => objective.Labels).FirstOrDefaultAsync(o => o.Title == spacis.Title 
                                                                      && o.Description == spacis.Description);
            if (result == null)
                db.Objectives.Add(spacis);
            else spacis = result;
        }
        
        var spacisTest = new Objective()
        {
            Id = Guid.NewGuid(),
            UserId = adminId,
            GroupId = study.Id,
            Title = "Spacis test",
            Description = "Test website for search bugs",
            DueDate = new DateTime(2024, 2, 28),
            StatusId = inProgress.Id,
            Priority = 100,
        };
        {
            var result = await db.Objectives.Include(objective => objective.Labels).FirstOrDefaultAsync(o => o.Title == spacisTest.Title 
                                                                      && o.Description == spacisTest.Description);
            if (result == null)
                db.Objectives.Add(spacisTest);
            else spacisTest = result;
        }

        Label learnLabel = new()
        {
            Id = Guid.NewGuid(),
            UserId = adminId,
            Name = "Learn",
        };
        {
            var result = await db.Labels.FirstOrDefaultAsync(l => l.Name == learnLabel.Name);
            if (result == null)
                db.Labels.Add(learnLabel);
            else learnLabel = result;
        }


        Label deadLineLabel = new()
        {
            Id = Guid.NewGuid(),
            UserId = adminId,
            Name = "Deadline",
        };
        {
            var result = await db.Labels.FirstOrDefaultAsync(l => l.Name == deadLineLabel.Name);
            if (result == null)
                db.Labels.Add(deadLineLabel);
            else deadLineLabel = result;
        }
        
        {
            if(!spacis.Labels.Contains(learnLabel))
                spacis.Labels.Add(learnLabel);
            if(!spacis.Labels.Contains(deadLineLabel))
                spacis.Labels.Add(deadLineLabel);
            if(!spacisTest.Labels.Contains(learnLabel))
                spacisTest.Labels.Add(learnLabel);
            await db.SaveChangesAsync();
        }
    }

    private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string testUserPw, User newUser)
    {
        var userManager = serviceProvider.GetService<UserManager<User>>();
        if (userManager == null)
            throw new Exception("userManager null");
        
        var user = await userManager.FindByNameAsync(newUser.UserName!);
        if (user != null)
            return user.Id;
        
        var result = await userManager.CreateAsync(newUser, testUserPw);
        if (!result.Errors.Any())
            return newUser.Id;
        
        var str = result.Errors.Aggregate("", (current, identityError) => 
            current + $"[{identityError}] {identityError.Description}\n");
        throw new Exception("The password is probably not strong enough!\n" + str);

    }

    private static async Task EnsureRole(IServiceProvider serviceProvider, string uid, string role)
    {
        var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
        if (roleManager == null)
            throw new Exception("roleManager null");

        if (!await roleManager.RoleExistsAsync(role)) 
            await roleManager.CreateAsync(new IdentityRole(role));
        
        if(string.IsNullOrEmpty(uid))
            return;
        
        var userManager = serviceProvider.GetService<UserManager<User>>();
        var user = await userManager!.FindByIdAsync(uid);
        if (user == null)
            throw new Exception("The testUserPw password was probably not strong enough!");

        await userManager.AddToRoleAsync(user, role);
    }
}