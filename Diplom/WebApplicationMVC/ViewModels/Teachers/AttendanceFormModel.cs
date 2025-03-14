namespace WebApplicationMVC.ViewModels.Teachers;

public class AttendanceFormModel
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; }
    public int LessonId { get; set; }
    public bool IsVisited { get; set; }
}