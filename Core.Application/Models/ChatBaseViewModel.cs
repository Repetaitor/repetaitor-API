namespace Core.Application.Models;

public class ChatBaseViewModel
{
    public int Id { get; set; }
    public string ChatName { get; set; } = "";
    public int ChatType { get; set; } = 0; // 0 - private, 1 - group
}