using Microsoft.AspNetCore.Mvc;
using WebApplicationMVC.Models.Database;

namespace WebApplicationMVC.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EventController : Controller
{
    //private readonly ApplicationDbContext _context;

    //public EventController(ApplicationDbContext context)
    //{
    //    _context = context;
    //}

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

        var events = new List<Event>();
        events.Add(new Event()
        {
            Id = 1,
            Title = "123",
            Start = DateTime.UtcNow,
            End = DateTime.UtcNow.AddHours(1),
            Color = GetGroupColor(1) // Назначение цвета
        });

        return Ok(events);
    }

    private string GetGroupColor(int groupId)
    {
        var colors = new Dictionary<int, string>
        {
            { 1, "#FF0000" }, // Красный
            { 2, "#008000" }, // Зеленый
            { 3, "#0000FF" }  // Синий
        };

        return colors.ContainsKey(groupId) ? colors[groupId] : "#808080"; // Серый по умолчанию
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
