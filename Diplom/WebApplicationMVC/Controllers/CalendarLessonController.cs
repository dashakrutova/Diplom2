using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationMVC.Models.Database;

namespace WebApplicationMVC.Controllers.Test;

[Route("[controller]")]
[ApiController]
[Authorize("admin")]
public class CalendarLessonController : Controller
{
    private readonly AppDbContext _context;

    public CalendarLessonController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("GetEvents")]
    public async Task<IActionResult> GetEvents(DateTime start, DateTime end)
    {
        var events = await _context
            .Lessons
            .Include(l => l.Group)
            .ThenInclude(g => g.Course)
            .Include(l => l.Group)
            .ThenInclude(g => g.Teacher)
            .Include(l => l.Group)
            .ThenInclude(g => g.Students)
            .Where(l => l.Start >= start && l.Start <= end)
            .Select(l => new Event()
            {
                Id = l.Id,
                Title = l.Group.Name + " \n " + l.Group.Teacher.LastName + " " + l.Group.Teacher.LastName[0] + ".",
                Start = l.Start,
                End = l.Start.AddHours(1),
                TeacherId = l.Group.TeacherId,
                GroupId = l.GroupId,
            })
            .ToListAsync();

        foreach (var item in events)
        {
            item.Color = GetGroupColor(item.GroupId);
        }

        return Ok(events);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] CreateEvent model)
    {
        if (model.Start == default || model.End == default)
            return BadRequest("Некорректные данные");

        var group = await _context.Groups
            .Include(g => g.Students)
            .FirstOrDefaultAsync(x => x.Id == model.GroupId);

        if (group == null)
                return NotFound("Группа не найдена");

        //var isAnyCrossingLessons = await _context
        //    .Lessons
        //    .Include(l => l.Group)
        //    .ThenInclude(g => g.Students)
        //    .Where(l => l.Group.TeacherId == group.TeacherId &&
        //        (l.Start < model.Start.AddMinutes(60) && l.Start.AddMinutes(60) > model.Start.AddMinutes(60) ||
        //        l.Start < model.Start && l.Start.AddMinutes(60) > model.Start))
        //    .AnyAsync();
        var isAnyCrossingLessons = await _context
            .Lessons
            .Include(l => l.Group)
            .ThenInclude(g => g.Students)
            .Where(l => l.Group.TeacherId == group.TeacherId &&
                (l.Start < model.End) && (l.Start.AddHours(1) > model.Start))
            .AnyAsync();



        if (isAnyCrossingLessons)
            return BadRequest("Ошибка: время занятий пересекаются");

        var lesson = new Lesson()
        {
            Group = group,
            Start = model.Start
        };

        _context.Add(lesson);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPut("Update")]
    public async Task<IActionResult> Update([FromBody] UpdateEvent model)
    {
        var existing = await _context
            .Lessons
            .Include(l => l.Group)
            .ThenInclude(g => g.Students)
            .FirstAsync(l => l.Id == model.Id);

        if (existing == null) 
            return NotFound();

        var group = await _context.Groups
            .Include(g => g.Students)
            .FirstOrDefaultAsync(x => x.Id == model.GroupId);

        if (group == null)
            return NotFound("Группа не найдена");

        //var isAnyCrossingLessons = await _context
        //        .Lessons
        //        .Include(l => l.Group)
        //        .ThenInclude(g => g.Students)
        //        .Where(l => l.Group.TeacherId == existing.Group.TeacherId &&
        //            l.Id != existing.Id &&
        //            (l.Start < model.Start.AddMinutes(60) && l.Start.AddMinutes(60) > model.Start.AddMinutes(60) ||
        //            l.Start < model.Start && l.Start.AddMinutes(60) > model.Start))
        //        .AnyAsync();

        var isAnyCrossingLessons = await _context
            .Lessons
            .Include(l => l.Group)
            .ThenInclude(g => g.Students)
            .Where(l => l.Group.TeacherId == existing.Group.TeacherId &&
                l.Id != existing.Id &&
                (l.Start < model.End) && (l.Start.AddHours(1) > model.Start))
            .AnyAsync();


        if (isAnyCrossingLessons)
            return BadRequest("Ошибка: время занятий пересекаются");

        existing.GroupId = model.GroupId;
        existing.Start = DateTime.SpecifyKind(model.Start, DateTimeKind.Local);

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var lesson = await _context.Lessons.FindAsync(id);
        if (lesson == null) 
            return NotFound();

        _context.Lessons.Remove(lesson);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("GetAll")]
    public async Task<List<Teacher>> GetAll()
    {
        var data = await _context
            .Users
            .Where(x => x.AppRole == AppRole.Teacher)
            .Include(u => u.Groups)
            .ThenInclude(g => g.Students)
            .Select(t => new Teacher
            {
                Id = t.Id,
                Name = t.FirstName,
                Groups = t.Groups.Select(g => new Group() { Id = g.Id, Name = g.Name }).ToList()
            })
            .ToListAsync();

        return data;
    }

    private string GetGroupColor(int groupId)
    {
        var colors = new Dictionary<int, string>
    {
        { 1, "#FF0000" }, // Красный
        { 2, "#008000" }, // Зеленый
        { 3, "#0000FF" }, // Синий
        { 4, "#FFA500" }, // Оранжевый
        { 5, "#800080" }, // Фиолетовый
        { 6, "#00FFFF" }, // Бирюзовый
        { 7, "#FFC0CB" }, // Розовый
        { 8, "#FFD700" }, // Золотой
        { 9, "#A52A2A" }, // Коричневый
        { 10, "#708090" }, // Серо-синий (slate gray)
        { 11, "#00FF00" }, // Ярко-зеленый
        { 12, "#000080" }, // Темно-синий
        { 13, "#DC143C" }, // Малиновый
        { 14, "#B22222" }, // Огоньный кирпич
        { 15, "#DA70D6" }, // Орхидея
        { 16, "#F0E68C" }, // Хаки
        { 17, "#7FFF00" }, // Ярко-салатовый
        { 18, "#20B2AA" }, // Светло-морская волна
        { 19, "#87CEFA" }, // Светло-голубой
        { 20, "#FF69B4" }, // Ярко-розовый
        { 21, "#6A5ACD" }, // Сизый
        { 22, "#48D1CC" }, // Средне-бирюзовый
        { 23, "#2E8B57" }, // Морская зелень
        { 24, "#D2691E" }, // Шоколадный
        { 25, "#C71585" }, // Темная орхидея
        { 26, "#40E0D0" }, // Бирюзовый
        { 27, "#FF6347" }, // Томатный
        { 28, "#008B8B" }, // Темно-бирюзовый
        { 29, "#191970" }, // Темно-синий (midnight blue)
        { 30, "#CD5C5C" }  // Индийский красный
    };

        return colors.TryGetValue(groupId, out var color) ? color : "#808080"; // Серый по умолчанию
    }
}

public class Event
{
    public int Id { get; set; }
    public string Title { get; set; } // Название события
    public DateTime Start { get; set; } // Дата и время начала
    public DateTime End { get; set; } // Дата и время окончания
    public int GroupId { get; set; } // ID группы для цвета
    public int TeacherId { get; set; }
    public string Color { get; set; }
}

public class CreateEvent
{
    public DateTime Start { get; set; } // Дата и время начала
    public DateTime End { get; set; } // Дата и время окончания
    public int GroupId { get; set; }
}

public class UpdateEvent
{
    public int Id { get; set; }
    public DateTime Start { get; set; } // Дата и время начала
    public DateTime End { get; set; } // Дата и время окончания
    public int GroupId { get; set; }
}

public class Teacher
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Group> Groups { get; set; }
}

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; }
}