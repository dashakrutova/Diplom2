namespace WebApplicationMVC.ViewModels.Teachers;

public class AddAttendanceFormModel
{
    public int LessonId { get; set; }
    public DateTime Date { get; set; }
    public string CourseName { get; set; }
    public string GroupName { get; set; }
    public List<AttendanceFormModel> Attendances { get; set; }
}

public class AttendanceFormModel
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; }
    public bool IsVisited { get; set; }
}