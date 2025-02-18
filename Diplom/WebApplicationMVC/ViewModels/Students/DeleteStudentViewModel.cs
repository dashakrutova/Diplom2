namespace WebApplicationMVC.ViewModels.Students;

public class DeleteStudentViewModel
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? MiddleName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public int GroupId { get; set; }
    public string GroupName { get; set; }
    public string ParentName { get; set; }
}