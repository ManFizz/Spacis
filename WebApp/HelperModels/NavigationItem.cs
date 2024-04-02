namespace WebApp.HelperModels;

public class NavigationItem
{
    public string Controller { get; set; }
    public string Action { get; set; }
    public string Icon { get; set; }
    public string Text { get; set; }
    public bool Display { get; set; }
}