using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Models;

public sealed class ApplicationContext : IdentityDbContext<User>
{
    public DbSet<Objective> Objectives { get; set; } = null!;
    public DbSet<Label> Labels { get; set; } = null!;
    public DbSet<Group> Groups { get; set; } = null!;
    public DbSet<Action> Actions { get; set; } = null!;
    public DbSet<Status> Statuses { get; set; } = null!;

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
        modelBuilder.ApplyConfiguration(new ObjectiveConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());

        #region TestData
        
        var admin = new User()
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "admin",
            PasswordHash = "admin",
        };
        modelBuilder.Entity<User>().HasData(admin);
        
        var study = new Group()
        {
            Id = Guid.NewGuid(),
            UserId = admin.Id,
            Name = "Study",
            Priority = 7
        };
        modelBuilder.Entity<Group>().HasData(study);
        
        var inProgress = new Status()
        {
            Id = Guid.NewGuid(),
            Name = "In progress"
        };
        modelBuilder.Entity<Status>().HasData(inProgress);
        
        var spacis = new Objective()
        {
            Id = Guid.NewGuid(),
            UserId = admin.Id,
            GroupId = study.Id,
            Title = "Spacis project",
            Description = "Create website on c# using ASP.NET and Entity Framework",
            DueDate = new DateTime(2024, 1, 14),
            StatusId = inProgress.Id,
            Priority = 99,
        };
        modelBuilder.Entity<Objective>().HasData(spacis);
        
        
        var spacisTest = new Objective()
        {
            Id = Guid.NewGuid(),
            UserId = admin.Id,
            GroupId = study.Id,
            Title = "Spacis test",
            Description = "Test website for search bugs",
            DueDate = new DateTime(2024, 2, 28),
            StatusId = inProgress.Id,
            Priority = 100,
        };
        modelBuilder.Entity<Objective>().HasData(spacisTest);
        
        var labels = new List<Label>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = admin.Id,
                Name = "Learn",
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = admin.Id,
                Name = "Deadline",
            }
        };
        modelBuilder.Entity<Label>().HasData(labels);
        
        // Записи для связи между Objective и Label
        var labelObjectives = labels.Select(label => new
        {
            LabelsId = label.Id,
            ObjectivesId = spacis.Id
        });

        modelBuilder.Entity("LabelObjective")
            .HasData(labelObjectives.ToArray());

        #endregion
        
        base.OnModelCreating(modelBuilder);
    }
    
    public class ObjectiveConfiguration : IEntityTypeConfiguration<Objective>
    {
        public void Configure(EntityTypeBuilder<Objective> builder)
        {
            builder.Property(o => o.Priority).HasDefaultValue(0);
            builder.Property(o => o.Description).HasDefaultValue("");
            builder.Property(o => o.DueDate).HasDefaultValueSql("GETDATE()");
            builder.HasOne(o => o.Group)
                .WithMany(g => g.Objectives)
                .HasForeignKey(o => o.GroupId);
            
            builder.HasOne(o => o.Status)
                .WithMany(s => s.Objectives)
                .HasForeignKey(o => o.StatusId);
        }
    }
    
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasMany(u => u.Labels)
                .WithOne(l => l.User)
                .HasForeignKey(l => l.UserId);
            
            builder.HasMany(u => u.Actions)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId);
            
            builder.HasMany(u => u.Groups)
                .WithOne(g => g.User)
                .HasForeignKey(g => g.UserId);
        }
    }
}