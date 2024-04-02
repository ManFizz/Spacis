using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class Permission
{
    [Key]
    public Guid Id { get; init; } = Guid.NewGuid();
    
    [StringLength(64)]
    public string CodeName { get; set; } = string.Empty;
    
    [StringLength(256)]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(256)]
    public string Info { get; set; } = string.Empty;

    public List<Role> Roles { get; set; } = [];
}