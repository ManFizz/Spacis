using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class Status
    {
        [Key]
        public Guid StatusId { get; set; }
        [Required]
        public string StatusName { get; set; }

        public List<Objective> Objectives { get; } = new();
    }
}