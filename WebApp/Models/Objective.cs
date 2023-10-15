using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace WebApp.Models
{
    public class Objective
    {
        [Key]
        public Guid ObjectiveId { get; set; }
        
        public Guid UserId { get; set; }
        public User User  { get; set; }
        
        public Guid GroupId { get; set; }
        public Group Group  { get; set; }
        
        [Required]
        public string Title { get; set; }
        
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        
        public Guid StatusId { get; set; }
        public Status Status  { get; set; }
        
        public int Priority { get; set; }
        
        public List<Label> Labels { get; set; } = new();
    }
}