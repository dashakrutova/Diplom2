using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplicationMVC.Auth;
using WebApplicationMVC.Models;
using WebApplicationMVC.Models.Database;
using WebApplicationMVC.Services;
using WebApplicationMVC.ViewModels.Users;

namespace WebApplicationMVC.Controllers;

public class AuthController : Controller
{
    private readonly UserManager _userManager;

    private readonly EmailService _emailService;

    public AuthController(UserManager userManager, EmailService emailService)
    {
        _userManager = userManager;
        _emailService = emailService;
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

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
    {
        var user = await _userManager.GetUserByEmailAsync(model.Login);
        if (user == null)
        {
            // Не палить наличие пользователя
            return RedirectToAction("ForgotPasswordConfirmation");
        }

        // Генерируем токен
        var token = Guid.NewGuid().ToString();
        user.PasswordResetToken = token;
        user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(1); // Токен живет 1 час
        await _userManager.UpdateUserAsync(user);

        // Здесь отправка email (псевдо-код):
        var resetLink = Url.Action("ResetPassword", "Auth", new { token = token }, Request.Scheme);
        await _emailService.SendResetPasswordEmail(user.Login, resetLink);


        return RedirectToAction("ForgotPasswordConfirmation");
    }

    [HttpGet]
    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ResetPassword(string token)
    {
        var model = new ResetPasswordModel { Token = token };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
    {
        var user = await _userManager.GetUserByResetTokenAsync(model.Token);

        if (user == null || user.PasswordResetTokenExpires < DateTime.UtcNow)
        {
            ModelState.AddModelError("", "Неверный или просроченный токен.");
            return View(model);
        }

        user.Password = model.NewPassword; // Здесь лучше хешировать пароль!
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpires = null;
        await _userManager.UpdateUserAsync(user);

        return RedirectToAction("Login");
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