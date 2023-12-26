using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers;

[Authorize(Roles=Constants.AdministratorsRole)]
public class ActionController(ApplicationContext db) : MainController(db)
{
    public Task<IActionResult> DisplayList()
    {
        return Task.FromResult<IActionResult>(View(db.Actions
            .Include(a => a.User)));
    }
}