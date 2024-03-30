using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels.Account;

public class LoginViewModel
{
    [Required(ErrorMessage = "Нужно ввести логин")]
    [Display(Name = "Логин")]
    public string Login { get; set; } = null!;
         
    [Required(ErrorMessage = "Нужно ввести пароль")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = null!;
         
    [Display(Name = "Запомнить?")]
    public bool RememberMe { get; set; }

    public string ReturnUrl { get; set; } = "";
}