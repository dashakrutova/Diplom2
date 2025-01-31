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
    [HttpGet]
    [Route("login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(LoginFormModel model)
    {
        User? user = UserManager.Login(model.Login, model.Password);
        if (user != null)
        {
            var identity = new ClaimsIdentity(user.Claims, AuthSettings.AuthCookieName);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(AuthSettings.AuthCookieName,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = false,
                });
            //HttpContext.Response.Cookies.Append("Abrakadabra", user.Login);
        }

        return RedirectToAction("Login", "Auth");
    }

    [HttpPost]
    [Route("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(AuthSettings.AuthCookieName);
        return RedirectToAction("Login", "Auth");
    }

    [Authorize]
    [HttpGet]
    [Route("onlyusers")]
    public IActionResult Onlyusers()
    {
        return View();
    }

    [Authorize("admin123")]
    [HttpGet]
    [Route("onlyadmins")]
    public IActionResult Onlyadmins()
    {
        return View();
    }

    public IActionResult Forbidden()
    {
        return View();
    }
}
