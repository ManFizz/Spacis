using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.HelperModels;
using WebApp.ViewModels;
using WebApp.ViewModels.User;

namespace WebApp.Controllers;

public class UserController(UserManager<User> userManager, ApplicationContext db) : Controller
{
    public async Task<IActionResult> Browse()
    {
        var users = await userManager.Users
            .Include(u => u.Members)
            .ToListAsync();
        
        return View(users);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);
        
        var user = new User { Email = model.Email, UserName = model.Login, DateOfBirth = model.DateOfBirth };
        var result = await userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
            return RedirectToAction("Browse");

        foreach (var error in result.Errors) 
            ModelState.AddModelError(string.Empty, error.Description);
        
        return View(model);
    }

    public async Task<IActionResult> Edit(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();
        
        var model = new EditUserViewModel {Id = user.Id, Email = user.Email!, Login = user.UserName!, DateOfBirth = user.DateOfBirth };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditUserViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);
        
        var user = await userManager.FindByIdAsync(model.Id);
        if (user == null)
            return View(model);
        
        user.Email = model.Email;
        user.UserName = model.Login;
        user.DateOfBirth = model.DateOfBirth;
                 
        var result = await userManager.UpdateAsync(user);
        if (result.Succeeded)
            return RedirectToAction("Browse");
        
        foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> Delete(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user != null)
        {
            var result = await userManager.DeleteAsync(user);
        }
        return RedirectToAction("Browse");
    }
    
    public async Task<IActionResult> ChangePassword(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();
        
        var model = new ChangePasswordViewModel { Id = user.Id, Login = user.UserName!};
        return View(model);
    }
 
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);
        
        var user = await userManager.FindByIdAsync(model.Id);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Пользователь не найден");
            return View(model);
        }

        var result =
            await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        if (result.Succeeded)
            return RedirectToAction("Browse");

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }
}