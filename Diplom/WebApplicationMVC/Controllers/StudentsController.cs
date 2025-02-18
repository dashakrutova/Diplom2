using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplicationMVC.Models.Database;
using WebApplicationMVC.ViewModels.Students;

namespace WebApplicationMVC.Controllers;

public class StudentsController : Controller
{
    private readonly AppDbContext _context;

    public StudentsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Students
    public async Task<IActionResult> Index()
    {
        var students = await _context
            .Students.Select(s => new StudentViewModel()
            {
                Id = s.Id,
                StudentName = s.FirstName + " " + s.LastName
            })
            .ToListAsync();

        return View(students);
    }

    // GET: Students/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var student = await _context.Students
            .Include(s => s.Group)
            .Include(s => s.Parent)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (student == null)
            return NotFound();

        var studentView = new StudentDetailsViewModel()
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            MiddleName = student.MiddleName,
            DateOfBirth = student.DateOfBirth,
            GroupId = student.GroupId,
            GroupName = student.Group.Name,
            ParentName = student.Parent.FirstName +
            " " + student.Parent.LastName + " " + student.MiddleName,

        };

        return View(studentView);
    }

    // GET: Students/Create
    public async Task<IActionResult> Create()
    {
        await SetParentsForViewBagAsync();
        await SetGroupsForViewBagAsync();
        return View();
    }

    // POST: Students/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateStudentFormModel model)
    {
        if (ModelState.IsValid)
        {
            var group = await _context
                .Groups
                .FirstOrDefaultAsync(x => x.Id == model.GroupId);

            if (group == null)
            {
                ModelState.AddModelError("GroupId", "Ошибка при выборе группы");
                await SetParentsForViewBagAsync(model.ParentId);
                await SetGroupsForViewBagAsync(model.GroupId);
                return View(model);
            }

            var parent = await _context
                .Users
                .Where(x => x.AppRole == AppRole.Parent)
                .FirstOrDefaultAsync(x => x.Id == model.ParentId);

            if (parent == null)
            {
                ModelState.AddModelError("ParentId", "Ошибка при выборе родителя");
                await SetParentsForViewBagAsync(model.ParentId);
                await SetGroupsForViewBagAsync(model.GroupId);
                return View(model);
            }

            var student = new Student()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                MiddleName = model.MiddleName,
                DateOfBirth = model.DateOfBirth,
                Group = group,
                Parent = parent
            };

            _context.Add(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        await SetParentsForViewBagAsync();
        await SetGroupsForViewBagAsync();
        return View(nameof(Create));
    }

    // GET: Students/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var student = await _context.Students.FindAsync(id);
        if (student == null)
            return NotFound();

        await SetGroupsForViewBagAsync(student.GroupId);
        await SetParentsForViewBagAsync(student.ParentId);

        var studentEditViewModel = new EditStudentFormModel()
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            MiddleName = student.MiddleName,
            DateOfBirth = student.DateOfBirth,
            GroupId = student.GroupId,
            ParentId = student.ParentId
        };

        return View(studentEditViewModel);
    }

    // POST: Students/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditStudentFormModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {

            try
            {
                var student = await _context
                    .Students
                    .FirstOrDefaultAsync(x => x.Id == model.Id);
                if (student == null)
                    return NotFound();

                var group = await _context
                .Groups
                .FirstOrDefaultAsync(x => x.Id == model.GroupId);

                if (group == null)
                {
                    ModelState.AddModelError("GroupId", "Ошибка при выборе группы");
                    await SetParentsForViewBagAsync(model.ParentId);
                    await SetGroupsForViewBagAsync(model.GroupId);
                    return View(model);
                }

                var parent = await _context
                    .Users
                    .Where(x => x.AppRole == AppRole.Parent)
                    .FirstOrDefaultAsync(x => x.Id == model.ParentId);

                if (parent == null)
                {
                    ModelState.AddModelError("ParentId", "Ошибка при выборе родителя");
                    await SetParentsForViewBagAsync(model.ParentId);
                    await SetGroupsForViewBagAsync(model.GroupId);
                    return View(model);
                }

                student.FirstName = model.FirstName;
                student.LastName = model.LastName;
                student.MiddleName = model.MiddleName;
                student.DateOfBirth = model.DateOfBirth;
                student.Group = group;
                student.Parent = parent;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await IsStudentExistAsync(model.Id))
                    return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

        await SetGroupsForViewBagAsync(model.GroupId);
        await SetParentsForViewBagAsync(model.ParentId);
        return View(model);
    }

    // GET: Students/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        // Todo: нужно проверять есть ли у студента уже посещения.
        // Если есть, то запретить удаление и вывести это на
        // страничке для пользователя
        var student = await _context.Students
            .Include(s => s.Group)
            .Include(s => s.Parent)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (student == null)
            return NotFound();

        var viewModel = new DeleteStudentViewModel()
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            MiddleName = student.MiddleName,
            DateOfBirth = student.DateOfBirth,
            GroupId = student.GroupId,
            GroupName = student.Group.Name,
            ParentName = student.Parent.FirstName +
            " " + student.Parent.LastName + " " + student.MiddleName,
        };

        return View(viewModel);
    }

    // POST: Students/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        // Todo: нужно проверять есть ли у студента уже посещения.
        // Если есть, то запретить удаление и вывести это на
        // страничке для пользователя
        var student = await _context.Students.FindAsync(id);

        if (student != null)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> IsStudentExistAsync(int id)
    {
        return await _context.Students.AnyAsync(e => e.Id == id);
    }

    private async Task SetGroupsForViewBagAsync(int? id = null)
    {
        var groups = await _context
            .Groups
            .Select(x => new { Id = x.Id, Name = x.Name })
            .ToListAsync();

        ViewBag.Groups = id == null
            ? new SelectList(groups, "Id", "Name")
            : new SelectList(groups, "Id", "Name", id);
    }

    private async Task SetParentsForViewBagAsync(int? id = null)
    {
        var parents = await _context
            .Users
            .Where(x => x.AppRole == AppRole.Parent)
            .Select(x => new 
            { 
                Id = x.Id, 
                Name = x.LastName + " " + x.FirstName + " " + x.MiddleName 
            })
            .ToListAsync();

        ViewBag.Parents = id == null
            ? new SelectList(parents, "Id", "Name")
            : new SelectList(parents, "Id", "Name", id);
    }
}
