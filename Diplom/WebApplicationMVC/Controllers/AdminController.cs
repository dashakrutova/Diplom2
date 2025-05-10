using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplicationMVC.Controllers;

[Authorize("admin")]
public class AdminController : Controller
{
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
}