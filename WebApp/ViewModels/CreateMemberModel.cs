using WebApp.Models;

namespace WebApp.ViewModels;

public class CreateMemberModel
{
    public string Name { get; set; }
    public string Info { get; set; }
    
    public Guid RoleId { get; set; } 
    public List<Role> Roles { get; set; } = []; 
}