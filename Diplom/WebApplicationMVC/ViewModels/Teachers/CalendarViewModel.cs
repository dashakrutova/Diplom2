namespace WebApplicationMVC.ViewModels.Teachers;

public class LessonViewModel
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string CourseName { get; set; }
    public string GroupName { get; set; }
    public string Teacher { get; set; }
    public string Notes { get; set; }
}

public class CalendarViewModel
{
    public int Year { get; set; }
    public int Month { get; set; }
    public List<LessonViewModel> Lessons { get; set; } = new List<LessonViewModel>();
    public List<DateTime> AlertDates { get; set; } = new List<DateTime>();
}