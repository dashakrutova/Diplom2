using System.ComponentModel.DataAnnotations;
namespace WebApplicationMVC.Models;

public class LoginFormModel
{
    [Required]
    [EmailAddress]
    public string Login { get; set; }

    [Required]
    public string Password { get; set; }
}