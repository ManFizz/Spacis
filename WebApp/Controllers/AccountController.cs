using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.HelperModels;
using WebApp.ViewModels.Account;

namespace WebApp.Controllers;

public class AccountController(UserManager<User> userManager, SignInManager<User> signInManager) : Controller
{
    public async Task<IActionResult> Register()
    {
        var currentUser = await userManager.GetUserAsync(User);
        if(currentUser != null)
            return RedirectToAction("Browse", "Objective");
        
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) 
            return View(model);
        
        var user = new User { Email = model.Email, UserName = model.Login, DateOfBirth = model.DateOfBirth};
        var result = await userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, Constants.UsersRole);
            await signInManager.SignInAsync(user, false);
            return RedirectToAction("Select", "Project");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }
    
    public async Task<IActionResult> Login(string returnUrl = "")
    {
        var currentUser = await userManager.GetUserAsync(User);
        if(currentUser != null)
            return RedirectToAction("Browse", "Objective");

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }
 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = 
            await signInManager.PasswordSignInAsync(model.Login, model.Password, model.RememberMe, false);
        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);
                
            return RedirectToAction("Browse", "Objective");
        }

        ModelState.AddModelError("", "Неправильный логин и (или) пароль");
        return View(model);
    }
 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }
}