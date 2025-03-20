namespace Core.Application.Models;

public class GroupBaseModal
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public string GroupName { get; set; }
    public string GroupCode { get; set; }
    public int StudentsCount { get; set; }
    public DateTime CreateDate { get; set; }
}