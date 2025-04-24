using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplicationMVC.Models.Database;
using WebApplicationMVC.ViewModels.Parents;

namespace WebApplicationMVC.Controllers;

[Authorize("parent")]
public class ParentController : Controller
{
    private readonly AppDbContext _context;

    public ParentController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> MyChilds()
    {

        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
            return BadRequest();

        var childs = await _context
            .Students
            .Where(s => s.UserId == userId)
            .Include(s => s.Group)
            .ThenInclude(g => g.Course)
            .Select(s => new ChildViewModel()
            {
                Id = s.Id,
                Name = s.FirstName,
                CourseName = s.Group.Course.Name
            })
            .ToListAsync();

        return View(childs);
    }

    // GET: User/ChildSchedule/5
    // https://localhost:7006/User/User%2FMyChilds?year=2024&month=2&yearChange=2024
    public async Task<IActionResult> ChildSchedule(int? id, 
        [FromQuery] int? year, [FromQuery] int? month)
    {

        if (year == null || month == null)
        {
            var now = DateTime.Now;
            year = now.Year;
            month = now.Month;
        }

        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
            return BadRequest();

        var student = await _context
            .Students
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

        if (student == null)
            return NotFound();

        var lessons = await _context
            .Lessons
            .Include(l => l.Group)
            .ThenInclude(l => l.Teacher)
            .Include(l => l.Group.Course)
            .Where(l => l.GroupId == student.GroupId && l.Start.Date.Year == year && l.Start.Date.Month == month)
            .Select(l => new LessonViewModel()
            {
                Date = l.Start,
                Teacher = l.Group.Teacher.LastName,
                CourseName = l.Group.Course.Name,
                Notes = string.Empty
            })
            .ToListAsync();

        var skippedLesonsDates = await _context
            .Attendances
            .Include(a => a.Lesson)
            .Where(a => a.StudentId == student.Id && !a.IsVisited)
            .Select(a => a.Lesson.Start.Date)
            .ToListAsync();

        var model = new CalendarViewModel
        {
            Year = (int)year,
            Month = (int)month,
            Lessons = lessons,
            AlertDates = skippedLesonsDates
        };

        return View(model);
    }
}