using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationMVC.Models.Database;

namespace WebApplicationMVC.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EventController : Controller
{
    private readonly AppDbContext _context;

    public EventController(AppDbContext context)
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
        //var events = await _context.Events
        //    .Where(e => e.Start >= start && e.End <= end)
        //    .Select(e => new
        //    {
        //        id = e.Id,
        //        title = e.Title,
        //        start = e.Start,
        //        end = e.End,
        //        color = GetGroupColor(e.GroupId) // Назначение цвета
        //    }).ToListAsync();

        //var events = new List<Event>();
        //events.Add(new Event()
        //{
        //    Id = 1,
        //    Title = "123",
        //    Start = DateTime.UtcNow,
        //    End = DateTime.UtcNow.AddHours(1),
        //    Color = GetGroupColor(1) // Назначение цвета
        //});

        List<Event> events;
        try
        {
            events = await _context
           .Lessons
           .Include(l => l.Group)
           .ThenInclude(g => g.Course)
           .Include(l => l.Group)
           .ThenInclude(g => g.Teacher)
           .Where(l => l.Start >= start && l.Start <= end)
           .Select(l => new Event()
           {
               Id = l.Id,
               Title = l.Group.Name + " \n " + l.Group.Teacher.LastName + " " + l.Group.Teacher.LastName[0] + ".",
               Start = l.Start,
               End = l.Start.AddHours(1),
               GroupId = l.GroupId,
           })
           .ToListAsync();
        }
        catch (Exception ex) 
        {

            throw;
        }

        foreach (var item in events)
        {
            item.Color = GetGroupColor(item.GroupId);
        }


        return Ok(events);
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
    public string Color { get; set; }
}
