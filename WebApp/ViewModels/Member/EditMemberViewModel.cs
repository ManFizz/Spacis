namespace WebApp.ViewModels.Member;

public class EditMemberViewModel
{
    public Guid Id { get; set; }
    
    public Guid ProjectId { get; set; }
    public Guid RoleId { get; set; } 
    public List<Models.User> Users { get; set; } = [];
    
    public string Name { get; set; }
    public string Info { get; set; }
    public List<Models.Role> Roles { get; set; } = [];

    public string UserId { get; set; }
    
}