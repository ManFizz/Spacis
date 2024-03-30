using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace WebApp.HelperModels;

public enum Color
{
    [Display(Name = "primary")]
    Primary,
    [Display(Name = "secondary")]
    Secondary,
    [Display(Name = "success")]
    Success,
    [Display(Name = "danger")]
    Danger,
    [Display(Name = "warning")]
    Warning,
    [Display(Name = "info")]
    Info,
    [Display(Name = "light")]
    Light,
    [Display(Name = "dark")]
    Dark
}

public static class ColorExtensions
{
    public static string GetDisplayName(Enum enumValue)
    {
        return enumValue.GetType()
            .GetMember(enumValue.ToString())
            .First()
            .GetCustomAttribute<DisplayAttribute>()
            ?.GetName() ?? enumValue.ToString();
    }

}
