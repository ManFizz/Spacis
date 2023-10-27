using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace WebApp.Models
{
    public class User : IdentityUser
    {
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        public List<Label> Labels { get; } = new();
        
        public List<Action> Actions { get; } = new();
        
        public List<Group> Groups { get; } = new();
    }
}