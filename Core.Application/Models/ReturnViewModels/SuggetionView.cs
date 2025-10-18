namespace Core.Application.Models.ReturnViewModels;

public class SuggetionView
{
    public bool NeedImprovement { get; set; } = true;
    public string SuggetionText { get; set; }
    public List<int> SuggetionValues { get; set; }
    public List<string> SuggetionCategories { get; set; }
}