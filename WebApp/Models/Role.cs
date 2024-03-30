using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class Role
{
    [Key]
    [ScaffoldColumn(false)]
    public Guid Id { get; init; } = Guid.NewGuid();
    
    [Required(ErrorMessage = "Обязательное поле")]
    [StringLength(256, MinimumLength = 6, ErrorMessage = "Длина названия должна быть от {2} до {1} символов")]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(256)]
    public string Info { get; set; } = string.Empty;
    
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public List<Permission> Permissions { get; set; } = [];
}