using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class Status
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }

        public List<Objective> Objectives { get; } = new();
    }
}