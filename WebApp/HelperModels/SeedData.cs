using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.HelperModels;

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
        
        await InitPermissions(context);
    }

    private static async Task InitPermissions(ApplicationContext db)
    {
        var permissions = new List<Permission>()
        {
            //Tasks
            new ()
            {
                CodeName = "create-task",
                Title = "Создание задач",
                Info = "Позволяет создовать новые задачи в проекте"
            },
            new ()
            {
                CodeName = "change-task",
                Title = "Изменение задачи",
                Info = "Позволяет редактировать информацию задачи в которой состоит участник"
            },
            new ()
            {
                CodeName = "change-task-all",
                Title = "Изменение всех задач",
                Info = "Позволяет редактировать информацию любой задачи (Требуется право \"Просмотр всех задач\")"
            },
            new ()
            {
                CodeName = "view-all-tasks",
                Title = "Просмотр всех задач",
                Info = "Позволяет просматривать все задачи проекта"
            },
            //Members
            new ()
            {
                CodeName = "view-members",
                Title = "Просмотр списка участников",
                Info = "Позволяет просмотреть список всех участников проекта"
            },
            new ()
            {
                CodeName = "add-member",
                Title = "Добавление участников",
                Info = "Позволяет добавлять участников в проект"
            },
            new ()
            {
                CodeName = "remove-member",
                Title = "Удалние участников",
                Info = "Позволяет удалять участников из проекта"
            },
            new ()
            {
                CodeName = "change-member",
                Title = "Изменение участников",
                Info = "Позволяет измененить информацию об участнике"
            },
            //Actions
            new ()
            {
                CodeName = "view-actions",
                Title = "Просмотр действий",
                Info = "Позволяет просмотреть историю действий в задачах в которых состоит участник"
            },
            new ()
            {
                CodeName = "view-actions-all",
                Title = "Просмотр всех действий",
                Info = "Позволяет просмотреть историю всех действий"
            },
            //Roles
            new ()
            {
                CodeName = "view-roles",
                Title = "Просмотр списка ролей",
                Info = "Позволяет просмотреть список всех ролей проекта"
            },
            new ()
            {
                CodeName = "add-role",
                Title = "Добавление ролей",
                Info = "Позволяет добавлять роли в проект"
            },
            new ()
            {
                CodeName = "remove-role",
                Title = "Удалние ролей",
                Info = "Позволяет удалять роли из проекта"
            },
            new ()
            {
                CodeName = "change-role",
                Title = "Изменение ролей",
                Info = "Позволяет измененить информацию о роли"
            },
            //Labels
            new ()
            {
                CodeName = "view-labels",
                Title = "Просмотр списка меток",
                Info = "Позволяет просмотреть список всех меток проекта"
            },
            new ()
            {
                CodeName = "add-label",
                Title = "Добавление меток",
                Info = "Позволяет добавлять метки в проект"
            },
            new ()
            {
                CodeName = "remove-label",
                Title = "Удалние меток",
                Info = "Позволяет удалять метки из проекта"
            },
            new ()
            {
                CodeName = "change-label",
                Title = "Изменение меток",
                Info = "Позволяет измененить информацию о метке"
            },
            //Statuses
            new ()
            {
                CodeName = "view-statuses",
                Title = "Просмотр полного списка статусов",
                Info = "Позволяет просмотреть список всех статусов проекта"
            },
            new ()
            {
                CodeName = "add-status",
                Title = "Добавление статусов",
                Info = "Позволяет добавлять статусы в проект"
            },
            new ()
            {
                CodeName = "remove-status",
                Title = "Удалние статусов",
                Info = "Позволяет удалять статусы из проекта"
            },
            new ()
            {
                CodeName = "change-status",
                Title = "Изменение статусов",
                Info = "Позволяет измененить информацию о статусе"
            },
        };
        
        var existingPermissions = await db.Permissions.ToListAsync();
        if (existingPermissions.Count != permissions.Count)
        {
            db.Permissions.RemoveRange(existingPermissions);
            db.Permissions.AddRange(permissions);
            await db.SaveChangesAsync();
        }
    }
    
    public static async Task InitNewProject(ApplicationContext dbContext, UserManager<User> userManager, User user, Project project)
    {
        await dbContext.Projects.AddAsync(project);
        
        var permissions = await dbContext.Permissions.ToListAsync();
        var adminRole = new Role()
        {
            Title = "Admin",
            Info = "Администратор проекта",
            Project = project,
            Permissions = permissions,
        };
        await dbContext.Roles.AddAsync(adminRole);
                
        var administrator = new Member()
        {
            Name = "Administrator",
            User = user,
            Role = adminRole,
            Project = project,
        };
        await dbContext.Members.AddAsync(administrator);
        
        user.SelectedMember = administrator;

        var statuses = new List<Status>
        {
            new()
            {
                Project = project,
                Title = "В планах",
                Color = Color.Primary,
            },
            new()
            {
                Project = project,
                Title = "В процессе",
                Color = Color.Info,
            },
            new()
            {
                Project = project,
                Title = "Готово",
                Color = Color.Success,
            },
            new()
            {
                Project = project,
                Title = "Брошено",
                Color = Color.Danger,
            },
            new()
            {
                Project = project,
                Title = "Отложено",
                Color = Color.Warning,
            },
            
        };
        await dbContext.Statuses.AddRangeAsync(statuses);

        var labels = new List<Label>()
        {
            new ()
            {
                Project = project,
                Title = "Срочно",
                Color = Color.Warning,
            },
            new ()
            {
                Project = project,
                Title = "Проблемы",
                Color = Color.Danger,
            },
        };
        await dbContext.Labels.AddRangeAsync(labels);
        
        await dbContext.SaveChangesAsync();
        await userManager.UpdateAsync(user);
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