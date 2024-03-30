using System.ComponentModel.DataAnnotations;
using WebApp.HelperModels;

namespace WebApp.Models
{
    public class Label
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;
        
        public Color Color { get; set; } = Color.Secondary;
        
        public Guid ProjectId { get; set; }
        public Project Project  { get; set; } = null!;
        
        public List<Objective> Objectives { get; } = [];
    }
}