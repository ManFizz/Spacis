using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using Action = WebApp.Models.Action;

namespace WebApp.HelperModels;

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
        base.OnModelCreating(modelBuilder);
        
        //Member x Objective
        modelBuilder.Entity<Objective>()
            .HasMany(o => o.Members)
            .WithMany(m => m.Objectives);
        
        //User x Member
        modelBuilder.Entity<User>()
            .HasMany(u => u.Members)
            .WithOne(m => m.User)
            .HasForeignKey(m => m.UserId);
        modelBuilder.Entity<User>()
            .HasOne(u => u.SelectedMember)
            .WithOne()
            .HasForeignKey<User>(u => u.SelectedMemberId)
            .IsRequired(false);

        //Role x Permission
        modelBuilder.Entity<Role>()
            .HasMany(r => r.Permissions)
            .WithMany(p => p.Roles);
        
        //Objective x Member
        modelBuilder.Entity<Objective>()
            .HasMany(o => o.Members)
            .WithMany(m => m.Objectives);
        modelBuilder.Entity<Objective>()
            .HasOne<Member>(o => o.Author)
            .WithMany(m => m.AuthorObjectives);
    }
}