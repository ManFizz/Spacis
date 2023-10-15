using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class Label
    {
        [Key]
        public Guid LabelId { get; set; }
        
        public Guid UserId { get; set; }
        public User User  { get; set; }
        
        [Required]
        public string LabelName { get; set; }
        
        public List<Objective> Objectives { get; set; } = new();
    }
}