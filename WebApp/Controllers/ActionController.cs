using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.SomeModels;

namespace WebApp.Controllers;

[Authorize(Roles=Constants.AdministratorsRole)]
public class ActionController(ApplicationContext db) : Controller
{
    public Task<IActionResult> DisplayList()
    {
        return Task.FromResult<IActionResult>(View(db.Actions
            .Include(a => a.Objective)
            .Include(a => a.Member)));
    }
}