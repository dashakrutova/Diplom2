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
    public async Task<User?> GetUserByLoginAsync(string login)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Login == login);
    }

    public bool VerifyPassword(User user, string password)
    {
        return user.Password == password;
    }

    //public async Task<User?> LoginAsync(string login, string password)
    //{
    //    return await _context
    //        .Users
    //        .FirstOrDefaultAsync(x => x.Login == login && x.Password == password);
    //}

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Login == email);
    }

    public async Task<User?> GetUserByResetTokenAsync(string token)
    {
        return await _context.Users.FirstOrDefaultAsync(x =>
            x.PasswordResetToken == token);
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
 
}