namespace WebApplicationMVC.Models.Database;

public class Student
{
	public int Id { get; set; }
	public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? MiddleName { get; set; }
    public DateOnly DateOfBirth { get; set; }
	public int GroupId { get; set; }
	public Group Group { get; set; }
	public int ParentId { get; set; }
	public User Parent { get; set; }
	public int? RoleId { get; set; }
	public Role? Role { get; set; }
}