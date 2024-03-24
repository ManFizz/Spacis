using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.SomeModels;

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