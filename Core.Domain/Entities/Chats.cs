namespace Core.Domain.Entities;

public class Chats
{
    public int Id { get; set; }
    public string ChatName { get; set; } = "";
    public int ChatType { get; set; } = 0; // 0 - private, 1 - group
}