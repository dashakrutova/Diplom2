using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace WebApplicationMVC.Models.Database;

public class User
{
	public int Id { get; set; }
	public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? MiddleName { get; set; }
    public string Login { get; set; }
	public string Password { get; set; }
	public string Number { get; set; }
	public DateOnly DateOfBirth { get; set; }
	public List<Student> Students { get; set; } = new List<Student>();// ???
    public List<Group> Groups { get; set; }

    public AppRole AppRole { get; set; }

	[NotMapped]
	public List<Claim> Claims
	{
		get
		{
            var claims = new List<Claim>();
			claims.Add(new Claim(ClaimTypes.Name, FirstName));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, Id.ToString()));

            if (AppRole == AppRole.Admin)
			{
				claims.Add(new Claim(AppRole.Admin.ToString(), "true"));
            }

			if (AppRole == AppRole.Parent)
			{
				claims.Add(new Claim(AppRole.Parent.ToString(), "true"));
            }

			if (AppRole == AppRole.Teacher)
			{
                claims.Add(new Claim(AppRole.Teacher.ToString(), "true"));
            }

			return claims;
		}
	}

	public string GetRole()
	{
		switch (AppRole)
		{
			case AppRole.Admin:
				return "Администратор";
            case AppRole.Teacher:
                return "Преподователь";
			case AppRole.Parent:
				return "Родитель";
			default:
				return "Неопределена";
        }
	}
}