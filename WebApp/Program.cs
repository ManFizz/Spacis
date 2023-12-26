using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

var builder = WebApplication.CreateBuilder(args);
var connection = builder.Configuration.GetConnectionString("DefaultConnection");
 
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlite(connection));

builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IPasswordValidator<User>,
    CustomPasswordValidator>(_ => new CustomPasswordValidator(6));
builder.Services.AddTransient<IUserValidator<User>, CustomUserValidator>();

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<User>>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAnonymousAccess", policy =>
    {
        policy.RequireAssertion(context => !context.User.Identity!.IsAuthenticated);
    });
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationContext>();
    context.Database.Migrate();

    var testUserPw = builder.Configuration.GetValue<string>("SeedUserPW");
    await SeedData.Initialize(services, testUserPw!);
}

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