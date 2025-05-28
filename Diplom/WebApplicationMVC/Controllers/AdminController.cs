using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationMVC.Models.Database;

namespace WebApplicationMVC.Controllers;

[Authorize("admin")]
public class AdminController : Controller
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
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

        //для отображения в ЛК
        if (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                ViewBag.Phone = user.Number;
                ViewBag.Email = user.Login;
                ViewBag.FullName = $"{user.LastName} {user.FirstName} {user.MiddleName}";
            }
        }

        return View();
    }
}