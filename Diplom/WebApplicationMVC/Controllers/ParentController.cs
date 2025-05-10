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
        var avatarDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "avatars");
        var allAvatars = Directory.GetFiles(avatarDir).Select(Path.GetFileName).ToList();
        var rng = new Random();
        ViewBag.Avatar = allAvatars.Any()
            ? "/images/avatars/" + allAvatars[rng.Next(allAvatars.Count)]
            : "/images/icon.png";

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

        var avatarDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "avatars");
        var allAvatars = Directory.GetFiles(avatarDir).Select(Path.GetFileName).ToList();
        var rng = new Random();

        foreach (var child in childs)
        {
            child.AvatarFileName = allAvatars[rng.Next(allAvatars.Count)];
        }

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

        //var student = await _context
        //    .Students
        //    .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

        var student = await _context
            .Students
            .Include(s => s.Group)
            .ThenInclude(g => g.Course)
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
                Teacher = l.Group.Teacher.LastName + " " + l.Group.Teacher.FirstName +
                          (l.Group.Teacher.MiddleName != null ? " " + l.Group.Teacher.MiddleName : ""),
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
            AlertDates = skippedLesonsDates,
            Balance = student.Balance,
            ChildFullName = $"{student.LastName} {student.FirstName} {student.MiddleName}",
            CourseName = student.Group.Course.Name
        };

        return View(model);
    }
}