using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplicationMVC.Models.Database;
using WebApplicationMVC.ViewModels.Teachers;

namespace WebApplicationMVC.Controllers;

[Authorize("teacher")]
public class TeacherController : Controller
{
    private readonly AppDbContext _context;

    public TeacherController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> MySchedule([FromQuery] int? year, [FromQuery] int? month)
    {
        if (year == null || month == null)
        {
            var now = DateTime.Now;
            year = now.Year;
            month = now.Month;
        }

        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
            return BadRequest();

        var lessons = await _context
           .Lessons
           .Include(l => l.Group)
           .ThenInclude(l => l.Teacher)
           .Include(l => l.Group.Course)
           .Where(l => l.Group.TeacherId == userId && l.Start.Date.Year == year && l.Start.Date.Month == month)
           .Select(l => new LessonViewModel()
           {
               Date = l.Start,
               Teacher = l.Group.Teacher.LastName,
               CourseName = l.Group.Course.Name,
               Notes = string.Empty
           })
           .ToListAsync();

        //var lessonForCopy = lessons.FirstOrDefault();
        //if (lessonForCopy != null)
        //{
        //    var lessonCopy = new LessonViewModel()
        //    {
        //        CourseName = "Абракадабра",
        //        Teacher = lessonForCopy.Teacher,
        //        Date = lessonForCopy.Date,
        //        Notes = "Notes"
        //    };
        //    lessons.Add(lessonCopy);
        //}

        var model = new CalendarViewModel
        {
            Year = (int)year,
            Month = (int)month,
            Lessons = lessons,
            AlertDates = new()
        };

        return View(model);
    }

    // Todo: нужен переход с календарного вида занятий на эту страницу
    // Todo: отображение на календаре занятий для который заполнена успеваемость
    // Todo: вернуться к календарному виду родителя и настроить отображение посещения ребенка
    // https://localhost:7006/Teacher/AddAtendances?id=2
    [HttpGet]
    public async Task<IActionResult> AddAtendances(int? id)
    {
        // int? id, studentId, studentName, true/false

        if (id == null)
            return NotFound();

        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
            return BadRequest();

        var lesson = await _context
            .Lessons
            .Include(l => l.Group)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (lesson == null)
            return NotFound();

        if (lesson.Group.TeacherId != userId)
            return BadRequest();

        var attendaces = await _context
            .Attendances
            .Include(a => a.Student)
            .Where(a => a.LessonId == lesson.Id)
            .Select(a => new AttendanceFormModel()
            {
                Id = a.Id,
                LessonId = a.LessonId,
                StudentId = a.StudentId,
                StudentName = a.Student.FirstName + " " + a.Student.LastName,
                IsVisited = a.IsVisited
            })
            .ToListAsync();

        if (!attendaces.Any())
        {
            var newAttendaces = await _context
                .Students
                .Where(s => s.GroupId == lesson.GroupId)
                .Select(s => new AttendanceFormModel()
                {
                   LessonId = lesson.Id,
                   StudentId = s.Id,
                   StudentName = s.FirstName + " " + s.LastName,
                })
                .ToArrayAsync();
            attendaces.AddRange(newAttendaces);
        }

        return View(attendaces);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAtendances(List<AttendanceFormModel> model)
    {
        if (model.Count == 0)
            return Index();

        if (ModelState.IsValid)
        {
            var attendancesForCreate = model.Where(a => a.Id == 0).ToList();
            var attendancesForUpdate = model.Where(a => a.Id != 0).ToList();

            if (attendancesForUpdate.Any())
            {
                var ids = attendancesForUpdate.Select(a => a.Id).ToList();
                var attendances = await _context
                    .Attendances
                    .Where(a => ids.Contains(a.Id))
                    .ToListAsync();

                foreach (var at in attendancesForUpdate)
                {
                    var updatingAttendance = attendances.FirstOrDefault(a => a.Id == at.Id);
                    if (updatingAttendance == null)
                    {
                        attendancesForCreate.Add(at);
                        continue;
                    }
                    updatingAttendance.IsVisited = at.IsVisited;
                }
            }

            foreach (var at in attendancesForCreate)
            {
                var attendance = new Attendance()
                {
                    StudentId = at.StudentId,
                    LessonId = at.LessonId,
                    IsVisited = at.IsVisited
                };
                _context.Attendances.Add(attendance);
            }

            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(MySchedule));
    }
}

// Календарь
// Мои группы

/*
 * Со страницы календаря
 * можем нажать на занятие, отобразиться подробная инфа + будет кнопка по которой можно 
 * перейти на страницу заполнения посещения за это занятие
 */
