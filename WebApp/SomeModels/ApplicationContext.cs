using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using Action = WebApp.Models.Action;

namespace WebApp.SomeModels;

public sealed class ApplicationContext : IdentityDbContext<User>
{
    public DbSet<Action> Actions { get; set; } = null!;
    public DbSet<Label> Labels { get; set; } = null!;
    public DbSet<Member> Members { get; set; } = null!;
    public DbSet<Objective> Objectives { get; set; } = null!;
    public DbSet<Permission> Permissions { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public new DbSet<Role> Roles { get; set; } = null!;
    public DbSet<Status> Statuses { get; set; } = null!;
    //Users in userManager

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
        //Database.EnsureDeleted();
        Database.EnsureCreated();
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Filename=MyDatabase.db");
        //optionsBuilder.LogTo(Console.WriteLine);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Objective>()
            .HasMany(o => o.Members)
            .WithMany(m => m.Objectives);
        /*modelBuilder.ApplyConfiguration(new ObjectiveConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new StatusConfiguration());*/
        
        base.OnModelCreating(modelBuilder);
    }
}