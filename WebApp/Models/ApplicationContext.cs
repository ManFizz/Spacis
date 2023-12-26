using Microsoft.EntityFrameworkCore.Metadata.Builders;
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