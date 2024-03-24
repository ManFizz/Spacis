using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class Role
{
    [Key]
    public Guid Id { get; init; } = Guid.NewGuid();
    
    [Required]
    [StringLength(256)]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(256)]
    public string Info { get; set; } = string.Empty;
    
    public Guid? ProjectId { get; set; }
    public Project? Project  { get; set; }

    public List<Permission> Permissions { get; } = [];
}