using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
 
namespace WebApp.Models
{
    public class CustomPasswordValidator(int length) : IPasswordValidator<User>
    {
        private int RequiredLength { get; } = length;

        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string? password)
        {
            var errors = new List<IdentityError>();
            if (string.IsNullOrEmpty(password))
            {
                errors.Add(new IdentityError
                {
                    Description = "Требуется указать пароль"
                });
            }
            else
            {
                if (password.Length < RequiredLength)
                {
                    errors.Add(new IdentityError
                    {
                        Description = $"Минимальная длина пароля равна {RequiredLength}"
                    });
                }

                const string pattern = "^[0-9a-zA-Z~`!@#$%^&*()_\\-+={[}]|\\:;\"'<,>.?/]+$";
                if (!Regex.IsMatch(password, pattern))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "Пароль может состоять только из букв, цифр и специальных символов"
                    });
                }
            }

            return Task.FromResult(errors.Count == 0 ?
                IdentityResult.Success : IdentityResult.Failed(errors.ToArray()));
        }
    }
}