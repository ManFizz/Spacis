using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class Objective
{
    [Key]
    public Guid Id { get; init; } = Guid.NewGuid();
        
    [Required]
    [StringLength(256)]
    public string Title { get; set; } = string.Empty;
        
    [StringLength(256)]
    public string Description { get; set; } = string.Empty;
        
    [Required]
    [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
    [DataType(DataType.DateTime)]
    public DateTime DueDateTime { get; set; }

    public int Priority { get; set; } = 0;
        
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
        
    public Guid StatusId { get; set; }
    public Status Status  { get; set; } = null!;
        
    public Guid AuthorId { get; set; }
    public Member Author { get; set; } = null!;
        
    public List<Label> Labels { get; set; } = [];
        
    public List<Member> Members { get; set; } = [];
}