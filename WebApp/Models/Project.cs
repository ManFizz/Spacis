using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class Project
{
    [Key]
    public Guid Id { get; init; } = Guid.NewGuid();

    [Required]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;
    
    [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
    [DataType(DataType.Date)]
    public DateTime DateCreated { get; set; } = DateTime.Now;

    public List<Member> Members { get; } = [];
    
    public List<Objective> Objectives { get; } = [];

    public List<Role> Roles { get; } = [];

    public List<Status> Statuses { get; } = [];

    public List<Label> Labels { get; } = [];
}