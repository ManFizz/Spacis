using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class Action
    {
        [Key]
        public Guid ActionId { get; set; }
        
        public Guid UserId { get; set; }
        public User User { get; set; }
        
        public DateTime DateTime { get; set; }
        [Required]
        public string ActionDescription { get; set; }
    }
}