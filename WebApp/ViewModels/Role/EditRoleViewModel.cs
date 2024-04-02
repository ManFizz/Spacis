using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels.Role;

public class EditRoleViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Обязательное поле")]
    [StringLength(256, MinimumLength = 3, ErrorMessage = "Длина названия должна быть от {2} до {1} символов")]
    public string Title { get; set; } = string.Empty;

    public string? Info { get; set; }

    public Guid ProjectId { get; set; }

    public List<AssignedPermissionData> Permissions { get; set; } = [];
}

public class AssignedPermissionData
{
    public Guid PermissionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool Assigned { get; set; }
}
