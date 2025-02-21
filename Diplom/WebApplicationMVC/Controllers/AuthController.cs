using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplicationMVC.Auth;
using WebApplicationMVC.Models;
using WebApplicationMVC.Models.Database;

namespace WebApplicationMVC.Controllers;

public class AuthController : Controller
{
    private readonly UserManager _userManager;

    public AuthController(UserManager userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginFormModel model)
    {
        User? user = await _userManager.LoginAsync(model.Login, model.Password);
        if (user == null)
        {
            ModelState.AddModelError("", "Неправильный email или пароль");
            return View(model);
        }

         var identity = new ClaimsIdentity(user.Claims, AuthSettings.AuthCookieName);

         var principal = new ClaimsPrincipal(identity);

         await HttpContext.SignInAsync(AuthSettings.AuthCookieName,
             principal,
             new AuthenticationProperties
             {
                 IsPersistent = false,
             });

         if (user.AppRole == AppRole.Admin)
             return RedirectToAction("Index", "Admin");

         return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(AuthSettings.AuthCookieName);
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpGet]
    public IActionResult Onlyusers()
    {
        return View();
    }

    [Authorize("admin")]
    [HttpGet]
    public IActionResult Onlyadmins()
    {
        return View();
    }

    public IActionResult Forbidden()
    {
        return View();
    }
}