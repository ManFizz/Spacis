using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers;

public class UserController(ApplicationContext db) : MainController(db)
{
    public Task<IActionResult> List()
    {
        ViewData["Title"] = "Users";
        return Task.FromResult<IActionResult>(View(db.Users));
    }
    
    public IActionResult Create()
    {
        ViewData["Title"] = "Create user";
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(User user)
    {
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return RedirectToAction("List");
    }
    
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null)
            return NotFound();

        var user = await db.Users.FirstOrDefaultAsync(p => p.UserId == id);
        if (user == null)
            return NotFound();
        
        Console.WriteLine(user.UserId);
        ViewData["Title"] = "Edit user";
        return View(user);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(User user)
    {
        Console.WriteLine(user.UserId);
        db.Users.Update(user);
        await db.SaveChangesAsync();
        return RedirectToAction("List");
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null)
            return NotFound();
        
        var user = new User { UserId = id.Value };
        db.Entry(user).State = EntityState.Deleted;
        await db.SaveChangesAsync();
        return RedirectToAction("List");
    }
}