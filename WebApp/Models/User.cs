using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }
        
        [Required]
        public string UserName { get; set; }
        [Required]
        public string PasswordHash  { get; set; }

        public List<Label> Labels { get; } = new();
        
        public List<Action> Actions { get; } = new();
        
        public List<Group> Groups { get; } = new();
    }
}