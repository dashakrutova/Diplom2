using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Diagnostics;
using WebApplicationMVC.Models;
using System;

namespace WebApplicationMVC.Controllers;

public class HomeController : Controller
{
	public IActionResult Index()
	{
		return View();
	}

	public IActionResult Privacy()
	{
		return View();
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
    [HttpPost]
    public async Task<IActionResult> SendMail(string fullName, string phone, string email)
    {
        var body = $@"
            <p><strong>���:</strong> {fullName}</p>
            <p><strong>�������:</strong> <a href='tel:{phone}'>{phone}</a></p>
            <p><strong>Email:</strong> <a href='mailto:{email}'>{email}</a></p>";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Lingvin", "krutova2021@yandex.ru"));
        message.To.Add(new MailboxAddress("�����", "krutova161616@gmail.com"));
        message.Subject = "����� ������ � �����";
        message.Body = new TextPart("html") { Text = body };
        var smtp_pass = Environment.GetEnvironmentVariable("SMTP_PASS", EnvironmentVariableTarget.User);
        using (var client = new SmtpClient())
        {
            await client.ConnectAsync("smtp.yandex.ru", 587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync("krutova2021@yandex.ru", smtp_pass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        return RedirectToAction("Index"); // ��� ������� View � ����������
    }

}