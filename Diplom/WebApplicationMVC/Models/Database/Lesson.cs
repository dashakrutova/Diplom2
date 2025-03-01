namespace WebApplicationMVC.Models.Database;

public class Lesson
{
	public int Id { get; set; }
	public int GroupId { get; set; }
	public Group Group { get; set; }
	public DateTime Start { get; set; }
}
