using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class Member
{
    [Key]
    public Guid Id { get; init; } = Guid.NewGuid();

    [StringLength(255)] 
    public string Name { get; set; } = string.Empty;

    [StringLength(255)] 
    public string Info { get; set; } = string.Empty;

    [StringLength(255)] 
    public string UserId { get; set; } = null!;
    public User User  { get; set; } = null!;
    
    public Guid ProjectId { get; set; }
    public Project Project  { get; set; } = null!;
    
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public List<Action> Actions { get; } = [];

    public List<Objective> Objectives { get; } = [];
    
    public List<Objective> AuthorObjectives { get; } = [];
}