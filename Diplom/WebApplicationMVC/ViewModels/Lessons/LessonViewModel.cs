namespace WebApplicationMVC.ViewModels.Lessons;

public class LessonViewModel
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public string GroupName { get; set; }
    public string TeacherName { get; set; }
    public DateTime Start { get; set; }
}