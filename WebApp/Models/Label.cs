using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class Label
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(32)]
        public string Color { get; set; } = string.Empty;
        
    
        public Guid ProjectId { get; set; }
        public Project Project  { get; set; } = null!;
        
        public List<Objective> Objectives { get; } = [];
    }
}