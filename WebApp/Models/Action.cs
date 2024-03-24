using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class Action
    {
        [Key] 
        public Guid Id { get; init; } = Guid.NewGuid();

        [StringLength(255)]
        public string Info { get; init; } = string.Empty;
        
        [Required]
        public int Type { get; init; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime DateTime { get; } = DateTime.Now;
        
        public Guid ObjectiveId { get; init; }
        public Objective Objective { get; init; } = null!;
        
        public Guid MemberId { get; init; }
        public Member Member { get; init; } = null!;
    }
}