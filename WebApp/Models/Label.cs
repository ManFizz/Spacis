using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class Label
    {
        [Key]
        public Guid Id { get; set; }
        
        public string UserId { get; set; }
        public User User  { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public List<Objective> Objectives { get; set; } = new();
    }
}