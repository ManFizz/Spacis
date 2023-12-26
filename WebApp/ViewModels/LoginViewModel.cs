using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Нужно ввести логин")]
    [Display(Name = "Логин")]
    public string? Login { get; set; }
         
    [Required(ErrorMessage = "Нужно ввести пароль")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string? Password { get; set; }
         
    [Display(Name = "Запомнить?")]
    public bool RememberMe { get; set; }

    public string ReturnUrl { get; set; } = "";
}