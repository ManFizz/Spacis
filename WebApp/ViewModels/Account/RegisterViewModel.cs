using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels.Account;

public class RegisterViewModel
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

    [Required]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    [DataType(DataType.Password)]
    [Display(Name = "Подтвердить пароль")]
    public string PasswordConfirm { get; set; }
}