using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; } = Guid.NewGuid();
        
        [Required]
        [StringLength(24, MinimumLength = 6)]
        public string Login { get; set; }
        
        [Required]
        [StringLength(24, MinimumLength = 6)]
        [Display(Name = "Password")]
        public string PasswordHash  { get; set; }

        public List<Label> Labels { get; } = new();
        
        public List<Action> Actions { get; } = new();
        
        public List<Group> Groups { get; } = new();
    }
}