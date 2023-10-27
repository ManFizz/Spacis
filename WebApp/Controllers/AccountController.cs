using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers;

public class AccountController(ApplicationContext db, UserManager<User> userManager, SignInManager<User> signInManager) : MainController(db)
{
    public IActionResult Register()
    {
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
            await signInManager.SignInAsync(user, false);
            return RedirectToAction("Index", "Index");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }
    
    public IActionResult Login(string returnUrl = "")
    {
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
                
            return RedirectToAction("Index", "Index");
        }

        ModelState.AddModelError("", "Неправильный логин и (или) пароль");
        return View(model);
    }
 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Index", "Index");
    }
}