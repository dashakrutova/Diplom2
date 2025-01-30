using System.Security.Claims;
using WebApplicationMVC.Models.Database;

namespace WebApplicationMVC.Auth;

public static class UserManager
{
    private static List<User> _users;

    static UserManager()
    {
        _users = new List<User>();

        _users.Add(new User()
        {
            Login = "admin@mail.ru",
            Password = "admin",
            Claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "admin"),
                new Claim("admin", "true")
            }
        });

        _users.Add(new User()
        {
            Login = "test@mail.ru",
            Password = "test",
            Claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "test")
            }
        });
    }

    public static User? Login(string login, string password)
    {
        return _users
            .FirstOrDefault(x => x.Login == login && x.Password == password);
    }
}