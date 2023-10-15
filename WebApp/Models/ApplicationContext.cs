using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApp.Models;

public sealed class ApplicationContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
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
        optionsBuilder.LogTo(Console.WriteLine);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ObjectiveConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());

        #region TestData
        
        var admin = new User()
        {
            UserId = Guid.NewGuid(),
            UserName = "admin",
            PasswordHash = "admin",
        };
        modelBuilder.Entity<User>().HasData(admin);
        
        var study = new Group()
        {
            GroupId = Guid.NewGuid(),
            UserId = admin.UserId,
            GroupName = "Study",
            Priority = 7
        };
        modelBuilder.Entity<Group>().HasData(study);
        
        var inProgress = new Status()
        {
            StatusId = Guid.NewGuid(),
            StatusName = "In progress"
        };
        modelBuilder.Entity<Status>().HasData(inProgress);
        
        var spacis = new Objective()
        {
            ObjectiveId = Guid.NewGuid(),
            UserId = admin.UserId,
            GroupId = study.GroupId,
            Title = "Spacis project",
            Description = "Create website on c# using ASP.NET and Entity Framework",
            DueDate = new DateTime(2024, 1, 14),
            StatusId = inProgress.StatusId,
            Priority = 99,
        };
        modelBuilder.Entity<Objective>().HasData(spacis);
        
        var labels = new List<Label>
        {
            new()
            {
                LabelId = Guid.NewGuid(),
                UserId = admin.UserId,
                LabelName = "Learn",
            },
            new()
            {
                LabelId = Guid.NewGuid(),
                UserId = admin.UserId,
                LabelName = "Deadline",
            }
        };
        modelBuilder.Entity<Label>().HasData(labels);
        
        // Записи для связи между Objective и Label
        var labelObjectives = labels.Select(label => new
        {
            LabelsLabelId = label.LabelId,
            ObjectivesObjectiveId = spacis.ObjectiveId
        });

        modelBuilder.Entity("LabelObjective")
            .HasData(labelObjectives.ToArray());

        #endregion
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