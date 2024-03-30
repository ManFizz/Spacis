using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using WebApp.Models;

namespace WebApp.HelperModels
{
    public class CustomUserValidator : UserValidator<User>
    {
        public override async Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
        {
            var errors = new List<IdentityError>();
            if (user.UserName!.Length < 4 || user.UserName.Length > 20)
            {
                errors.Add(new IdentityError
                {
                    Description = "Имя пользователя должно быть от 4 до 20 символов в длину."
                });
            }

            var existingUser = await manager.FindByNameAsync(user.UserName);
            if (existingUser != null && !string.Equals(existingUser.Id, user.Id, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add(new IdentityError
                {
                    Description = "Login пользователя уже используется."
                });
            }

            var existingEmail = await manager.FindByEmailAsync(user.Email!);
            if (existingEmail != null && !string.Equals(existingEmail.Id, user.Id, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add(new IdentityError
                {
                    Description = "Адрес электронной почты уже используется."
                });
            }

            if (!Regex.IsMatch(user.UserName, "^[a-zA-Z0-9_-]+$"))
            {
                errors.Add(new IdentityError
                {
                    Description = "Имя пользователя может содержать только буквы, цифры, дефис и подчеркивание."
                });
            }

            var forbiddenWords = new List<string> { "admin", "superuser", "root" };
            if (forbiddenWords.Any(word => user.UserName.Contains(word, StringComparison.OrdinalIgnoreCase)) 
                && !user.UserName.Equals(Constants.AdministratorsRole))
            {
                errors.Add(new IdentityError
                {
                    Description = "Имя пользователя содержит запрещенные слова."
                });
            }
            
            var minDateOfBirth = DateTime.Now.AddYears(-14);
            if (user.DateOfBirth > minDateOfBirth)
            {
                errors.Add(new IdentityError
                {
                    Description = "Пользователи должны быть старше 14 лет."
                });
            }
            
            var disposableEmailDomains = new List<string> { "example.com", "disposable.com" };
            if (disposableEmailDomains.Any(domain => user.Email.EndsWith("@" + domain, StringComparison.OrdinalIgnoreCase)))
            {
                errors.Add(new IdentityError
                {
                    Description = "Использование одноразовых адресов электронной почты недопустимо."
                });
            }

            return errors.Count == 0 ?
                IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
        }
    }
}
