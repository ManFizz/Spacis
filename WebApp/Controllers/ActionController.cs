using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers;

public class ActionController(ApplicationContext db) : MainController(db)
{
    public Task<IActionResult> ViewActions()
    {
        ViewData["Title"] = "Actions";
        return Task.FromResult<IActionResult>(View(db.Actions
            .Include(a => a.User)));
    }
}