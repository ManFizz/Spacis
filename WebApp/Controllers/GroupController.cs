using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers;

public class GroupController(ApplicationContext db) : MainController(db)
{
    public Task<IActionResult> DisplayList()
    {
        ViewData["Title"] = "Groups";
        return Task.FromResult<IActionResult>(View(db.Groups
            .Include(g => g.User)
            .Include(g => g.Objectives)));
    }
}