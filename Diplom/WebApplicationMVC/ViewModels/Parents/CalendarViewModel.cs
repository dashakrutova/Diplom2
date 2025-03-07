namespace WebApplicationMVC.ViewModels.Parents;

//public class CalendarViewModel
//{
//    //public int Year { get; set; }
//    //public int Month { get; set; }
//    //public List<DateTime> LessonDates { get; set; } = new List<DateTime>();
//    //public List<DateTime> AlertDates { get; set; } = new List<DateTime>();

//}

public class LessonViewModel
{
    public DateTime Date { get; set; }
    public string CourseName { get; set; }
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