using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers;

public class RoleController(ApplicationContext db, RoleManager<IdentityRole> roleManager, UserManager<User> userManager) : MainController(db)
{
        public IActionResult DisplayList() => View(roleManager.Roles.ToList());
 
        public IActionResult Create() => View();
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            if (string.IsNullOrEmpty(name))
                return View(name);
            
            var result = await roleManager.CreateAsync(new IdentityRole(name));
            if (result.Succeeded)
                return RedirectToAction("DisplayList");

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            
            return View(name);
        }
         
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role != null)
            {
                var result = await roleManager.DeleteAsync(role);
            }
            return RedirectToAction("DisplayList");
        }
 
        public IActionResult UserList() => View(userManager.Users.ToList());
 
        public async Task<IActionResult> Edit(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();
            
            var userRoles = await userManager.GetRolesAsync(user);
            var allRoles = roleManager.Roles.ToList();
            var model = new ChangeRoleViewModel
            {
                UserId = user.Id,
                UserEmail = user.Email!,
                UserRoles = userRoles,
                AllRoles = allRoles
            };
            return View(model);

        }
        [HttpPost]
        public async Task<IActionResult> Edit(string userId, List<string> roles)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();
            
            var userRoles = await userManager.GetRolesAsync(user);
            var addedRoles = roles.Except(userRoles);
            var removedRoles = userRoles.Except(roles);
            
            await userManager.AddToRolesAsync(user, addedRoles);
            await userManager.RemoveFromRolesAsync(user, removedRoles);
 
            return RedirectToAction("UserList");

        }
}