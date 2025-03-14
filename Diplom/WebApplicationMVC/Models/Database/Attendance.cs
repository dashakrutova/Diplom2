namespace WebApplicationMVC.Models.Database;

public class Attendance
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public Lesson Lesson { get; set; }
    public int StudentId { get; set; }
    public Student Student { get; set; }
    public bool IsVisited { get; set; }
}
