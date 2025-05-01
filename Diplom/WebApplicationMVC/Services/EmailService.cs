using MailKit.Net.Smtp;
using MimeKit;

namespace WebApplicationMVC.Services;

public class EmailService
{
    private readonly string _smtpServer = "smtp.yandex.ru";
    private readonly int _smtpPort = 587;
    private readonly string _smtpUser = "krutova2021@yandex.ru";
    private readonly string _smtpPass = "lwgzpvuosujyrbvv";

    public async Task SendResetPasswordEmail(string toEmail, string resetLink)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Lingvin", _smtpUser));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = "Сброс пароля";

        var builder = new BodyBuilder();

        builder.HtmlBody = $@"
        <div style='font-family: Arial, sans-serif; font-size: 16px; color: #333;'>
            <h2>Сброс пароля</h2>
            <p>Вы запросили сброс пароля на нашем сайте.</p>
            <p>Чтобы сбросить пароль, нажмите на кнопку ниже:</p>
            <p>
                <a href='{resetLink}' style='
                    display: inline-block;
                    padding: 10px 20px;
                    font-size: 16px;
                    color: white;
                    background-color: #007bff;
                    text-decoration: none;
                    border-radius: 5px;
                '>Сбросить пароль</a>
            </p>
            <p>Если вы не запрашивали сброс пароля, просто проигнорируйте это письмо.</p>
            <p>С уважением, школа английского языка</p>
        </div>";

        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_smtpUser, _smtpPass);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
