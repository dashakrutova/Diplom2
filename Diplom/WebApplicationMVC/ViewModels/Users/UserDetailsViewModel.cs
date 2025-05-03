namespace WebApplicationMVC.ViewModels.Users;

public class UserDetailsViewModel
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? MiddleName { get; set; }
    public string Login { get; set; }
    public string Number { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string AppRole { get; set; }
    public bool IsParent => AppRole == "Родитель";
    public bool IsTeacher => AppRole == "Преподователь";
}