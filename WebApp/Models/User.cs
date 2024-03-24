using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WebApp.Models
{
    public class User : IdentityUser
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime DateJoin { get; set; } = DateTime.Now;
        
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        
        public List<Member> Members { get; } = [];
        
        public Guid? SelectedProjectId { get; set; }
        public Project? SelectedProject { get; set; }
    }
}