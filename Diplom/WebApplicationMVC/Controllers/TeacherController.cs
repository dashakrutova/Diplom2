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
        var avatarDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "avatars");
        var allAvatars = Directory.GetFiles(avatarDir).Select(Path.GetFileName).ToList();
        var rng = new Random();
        ViewBag.Avatar = allAvatars.Any()
            ? "/images/avatars/" + allAvatars[rng.Next(allAvatars.Count)]
            : "/images/icon.png";

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
           .Include(l => l.Group)
           .ThenInclude(l => l.Students)
           .Include(l => l.Group.Course)
           .Where(l => l.Group.TeacherId == userId && l.Start.Date.Year == year && l.Start.Date.Month == month)
           .Select(l => new LessonViewModel()
           {
               Id = l.Id,
               Date = l.Start,
               Teacher = l.Group.Teacher.LastName,
               CourseName = l.Group.Course.Name,
               GroupName = l.Group.Name,
               Notes = string.Empty
           })
           .ToListAsync();

        var lessonIds = lessons.Select(x => x.Id);

        // Асинхронно получаем массив уникальных идентификаторов уроков, 
        // у которых есть связанные записи в таблице Attendance.
        var lessonIdsWithAttendance = await _context
            .Attendances // Обращаемся к таблице посещаемости (Attendance)
            .Where(a => lessonIds.Contains(a.LessonId)) // Фильтруем записи посещаемости, 
             // оставляя только те, у которых LessonId содержится в списке идентификаторов уроков (lessons).
            .Select(a => a.LessonId) // Из отфильтрованных записей выбираем только поле LessonId.
            .Distinct() // Оставляем только уникальные значения LessonId (убираем дубликаты).
            .ToArrayAsync(); // Выполняем запрос к БД асинхронно и преобразуем результат в массив.

        var datesOfLessonsWithoutAttendaces = lessons
            .Where(l => !lessonIdsWithAttendance.Contains(l.Id))
            .Select(l => l.Date.Date)
            .ToList();

        var model = new CalendarViewModel
        {
            Year = (int)year,
            Month = (int)month,
            Lessons = lessons,
            AlertDates = datesOfLessonsWithoutAttendaces
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> AddAttendances(int? id)
    {
        if (id == null)
            return NotFound();

        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
            return BadRequest();

        var lesson = await _context
            .Lessons
            .Include(l => l.Group)
            .ThenInclude(g => g.Course)
            .Include(l => l.Group)
            .ThenInclude(g => g.Students)
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
                   StudentId = s.Id,
                   StudentName = s.FirstName + " " + s.LastName,
                })
                .ToArrayAsync();
            attendaces.AddRange(newAttendaces);
        }

        var model = new AddAttendanceFormModel()
        {
            LessonId = lesson.Id,
            GroupName = lesson.Group.Name,
            Date = lesson.Start,
            CourseName = lesson.Group.Course.Name,
            Attendances = attendaces ?? new List<AttendanceFormModel>()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAttendances(AddAttendanceFormModel model)
    {
        if (!ModelState.IsValid || model.Attendances.Count == 0)
            return RedirectToAction(nameof(MySchedule));

        var lesson = await _context.Lessons
            .Include(l => l.Group)
                .ThenInclude(g => g.Course)
            .FirstOrDefaultAsync(l => l.Id == model.LessonId);
        if (lesson == null) return BadRequest();

        var course = lesson.Group.Course;
        int price = lesson.Group.GroupType == GroupType.Personal
            ? course.IndividualPrice
            : course.GroupPrice;

        // 1) Обновление существующих записей
        var toUpdate = model.Attendances.Where(a => a.Id != 0).ToList();
        if (toUpdate.Any())
        {
            var exist = await _context.Attendances
                .Where(a => toUpdate.Select(vm => vm.Id).Contains(a.Id))
                .ToListAsync();

            foreach (var vm in toUpdate)
            {
                var dbA = exist.First(a => a.Id == vm.Id);

                // если из false→true, и ещё не списывали — списать и пометить IsCharged
                if (!dbA.IsVisited && vm.IsVisited && !dbA.IsCharged)
                {
                    var student = await _context.Students.FindAsync(vm.StudentId);
                    student.Balance -= price;

                    dbA.IsCharged = true;
                }

                // просто обновляем флаг посещения
                dbA.IsVisited = vm.IsVisited;
            }
        }

        // 2) Создание новых записей
        var toCreate = model.Attendances.Where(a => a.Id == 0).ToList();
        foreach (var vm in toCreate)
        {
            var attendance = new Attendance
            {
                StudentId = vm.StudentId,
                LessonId = model.LessonId,
                IsVisited = vm.IsVisited,
                // если сразу стоит галочка — списываем и отмечаем
                IsCharged = vm.IsVisited
            };
            _context.Attendances.Add(attendance);

            if (vm.IsVisited)
            {
                var student = await _context.Students.FindAsync(vm.StudentId);
                student.Balance -= price;
            }
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(MySchedule));
    }
}