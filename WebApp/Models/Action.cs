using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class Action
    {
        [Key]
        public Guid Id { get; set; }
        
        public string UserId { get; set; }
        public User User { get; set; }
        
        public DateTime DateTime { get; set; }
        [Required]
        public string Description { get; set; }
    }
}