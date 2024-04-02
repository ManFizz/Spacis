using WebApp.Models;
using WebApp.Models;

namespace WebApp.ViewModels.Objective;

public class ObjectiveCreateViewModel
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DueDateTime { get; set; }
    
    public List<Guid> SelectedLabelIds { get; set; } = [];
    public List<Label> AvailableLabels { get; set; } = [];
    
    public List<Guid> SelectedMemberIds { get; set; } = [];
    public List<Models.Member> AvailableMembers { get; set; } = [];
}
