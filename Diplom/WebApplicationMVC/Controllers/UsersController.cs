using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplicationMVC.Models.Database;
using WebApplicationMVC.ViewModels.Users;

namespace WebApplicationMVC.Controllers;

[Authorize("admin")]
public class UsersController : Controller
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Users
    public async Task<IActionResult> Index()
    {
        var users = await _context
            .Users
            .Select(u => new UserViewModel()
            {
                Id = u.Id,
                UserName = u.FirstName + " " + u.LastName + " " + u.MiddleName,
                AppRole = u.GetRole()
            })
            .ToListAsync();
        return View(users);
    }

    // GET: Users/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var user = await _context.Users
            .Select(u => new UserDetailsViewModel()
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                MiddleName = u.MiddleName,
                Login = u.Login,
                Number = u.Number,
                DateOfBirth = u.DateOfBirth,
                AppRole = u.GetRole()
            })
            .FirstOrDefaultAsync(m => m.Id == id);
        if (user == null)
            return NotFound();

        return View(user);
    }

    // GET: Users/Create
    public IActionResult Create()
    {
        SetAppRoles();
        return View();
    }

    // POST: Users/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserFormModel model)
    {
        if (await _context.Users.AnyAsync(u => u.Login == model.Login))
        {
            ModelState.AddModelError("Login", "This login is already taken.");
            SetAppRoles(model.AppRole);
            return View(model);
        }

        if (ModelState.IsValid)
        {
            var user = new User()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                MiddleName = model.MiddleName,
                Login = model.Login,
                Number = model.Number,
                Password = model.Password,
                DateOfBirth = DateOnly.FromDateTime(model.DateOfBirth),
                AppRole = (AppRole)model.AppRole
            };
            _context.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        SetAppRoles(model.AppRole);
        return View(model);
    }

    // GET: Users/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        var model = new EditUserFormModel()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MiddleName = user.MiddleName,
            Login = user.Login,
            Number = user.Number,
            DateOfBirth = user.DateOfBirth.ToDateTime(TimeOnly.MinValue)
        };

        return View(model);
    }

    // POST: Users/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditUserFormModel model)
    {
        if (id != model.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                var user = await _context
                    .Users
                    .FirstOrDefaultAsync(u => u.Id == model.Id && u.Login == model.Login);

                if (user == null)
                    return NotFound();

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.MiddleName = model.MiddleName;
                user.Number = model.Number;
                user.DateOfBirth = DateOnly.FromDateTime(model.DateOfBirth);

                if (!string.IsNullOrEmpty(model.Password))
                    user.Password = model.Password;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(model.Id))
                    return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    // GET: Users/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var user = await _context.Users
            .FirstOrDefaultAsync(m => m.Id == id);

        if (user == null)
            return NotFound();

        var model = new DeleteUserFormModel()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MiddleName = user.MiddleName,
            Login = user.Login,
            Number = user.Number,
            DateOfBirth = user.DateOfBirth,
            AppRole = user.GetRole()
        };

        return View(model);
    }

    // POST: Users/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user != null)
            _context.Users.Remove(user);
        

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool UserExists(int id)
    {
        return _context.Users.Any(e => e.Id == id);
    }

    private void SetAppRoles(int? id = null)
    {
        var appRoles = new List<(string, int)>();
        appRoles.Add(new("Администратор", 1));
        appRoles.Add(new("Преподователь", 2));
        appRoles.Add(new("Родитель", 3));

        var roles = appRoles.Select(x => new { Id = x.Item2, Name = x.Item1 });

        ViewBag.AppRoles = id == null
            ? new SelectList(roles, "Id", "Name")
            : new SelectList(roles, "Id", "Name", id);
    }
}
