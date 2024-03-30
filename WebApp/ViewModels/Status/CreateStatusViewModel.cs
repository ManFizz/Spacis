using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels;

public class CreateStatusViewModel
{
    [StringLength(255, MinimumLength = 3)] 
    public string Title { get; set; } = null!;
        
    [StringLength(32)] 
    public string Color { get; set; } = string.Empty;
    
    public Guid ProjectId { get; set; }
}