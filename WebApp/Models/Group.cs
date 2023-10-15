using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class Group
    {
        [Key]
        public Guid GroupId { get; set; }
        
        public Guid UserId { get; set; }
        public User User  { get; set; }
        
        [Required]
        public string GroupName { get; set; }
        
        public int Priority { get; set; }
        
        public List<Objective> Objectives { get; } = new();
    }
}