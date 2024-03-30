using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels.User;

public class CreateUserViewModel
{
    [Required]
    [Display(Name = "Email")]
    public string Email { get; set; }
    
    [Required]
    [Display(Name = "Login")]
    public string Login { get; set; }
    
    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Дата рождения")]
    [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; }
}