using Microsoft.EntityFrameworkCore;
using WebApplicationMVC.Models.Database;

namespace WebApplicationMVC.Auth;

public class UserManager
{
    private readonly AppDbContext _context;

    public UserManager(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> LoginAsync(string login, string password)
    {
        return await _context
            .Users
            .FirstOrDefaultAsync(x => x.Login == login && x.Password == password);
    }
}