using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
 
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlite(connection));

builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IPasswordValidator<User>,
    CustomPasswordValidator>(serv => new CustomPasswordValidator(6));
builder.Services.AddTransient<IUserValidator<User>, CustomUserValidator>();

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationContext>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Index}/{action=Index}/{id?}");

app.Run();