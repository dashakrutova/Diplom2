using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplicationMVC.Models.Database;
using WebApplicationMVC.ViewModels.Groups;

namespace WebApplicationMVC.Controllers;

[Authorize("admin")]
public class GroupsController : Controller
{
    private readonly AppDbContext _context;

    public GroupsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Groups
    public async Task<IActionResult> Index()
    {
        var grps = await _context
            .Groups
            .Include(g => g.Students)
            .Include(g => g.Course)
            .Include(g => g.Teacher)
            .ToListAsync(); ;

        var groups = await _context
            .Groups
            .Include(g => g.Students)
            .Include(g => g.Course)
            .Include(g => g.Teacher)
            .Select(g => new GroupViewModel()
            {
                Id = g.Id,
                Name = g.Name,
                CourseName = g.Course.Name,
                TeacherName = g.Teacher.FirstName,
            })
            .ToListAsync();

        return View(groups);
    }

    // GET: Groups/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var group = await _context
            .Groups
            .Include(g => g.Students)
            .Include(g => g.Course)
            .Include(g => g.Teacher)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (group == null)
            return NotFound();

        var gropViewModel = new GroupViewModel()
        {
            Id = group.Id,
            Name = group.Name,
            CourseName = group.Course.Name,
            TeacherName = group.Teacher.FirstName,
        };

        return View(gropViewModel);
    }

    // GET: Groups/Create
    public async Task<IActionResult> Create()
    {
        await SetCoursesForViewBagAsync();
        await SetTeachersForViewBagAsync();
        return View();
    }

    // POST: Groups/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateGroupFormModel model)
    {
        if (ModelState.IsValid)
        {
            var teacher = await _context
                .Users
                .FirstOrDefaultAsync(x => x.Id == model.TeacherId && x.AppRole == AppRole.Teacher);

            if (teacher == null || teacher.AppRole != AppRole.Teacher)
            {
                ModelState.AddModelError("TeacherId", "Ошибка при выборе преподователя");
                await SetTeachersForViewBagAsync(model.TeacherId);
                await SetCoursesForViewBagAsync(model.CourseId);
                return View(model);
            }

            var course = await _context
                .Courses
                .FirstOrDefaultAsync(x => x.Id == model.CourseId);

            if (course == null)
            {
                ModelState.AddModelError("CourseId", "Ошибка при выборе курса");
                await SetTeachersForViewBagAsync(model.TeacherId);
                await SetCoursesForViewBagAsync(model.CourseId);
                return View(model);
            }
                
            var group = new Group()
            {
                Name = model.Name,
                Course = course,
                Teacher = teacher,
            };

            _context.Add(group);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return RedirectToAction(nameof(Create));
    }

    // GET: Groups/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var group = await _context
            .Groups
            .Include(g => g.Students)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (group == null)
            return NotFound();

        var groupEditModel = new EditGroupFormModel()
        {
            Id = group.Id,
            Name = group.Name,
            CourseId = group.CourseId,
            TeacherId = group.TeacherId,
        };

        await SetCoursesForViewBagAsync(group.CourseId);
        await SetTeachersForViewBagAsync(group.TeacherId);
        return View(groupEditModel);
    }

    // POST: Groups/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditGroupFormModel model)
    {
        if (id != model.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                var group = await _context.Groups.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (group == null)
                    return NotFound();

                var course = await _context
                   .Courses
                   .FirstOrDefaultAsync(x => x.Id == model.CourseId);

                if (course == null)
                    // Todo: по хорошему нужно дать нормально понять пользователю, что указанный user не может быть учителем
                    return NotFound();

                var teacher = await _context
                    .Users
                    .FirstOrDefaultAsync(x => x.Id == model.TeacherId && x.AppRole == AppRole.Teacher);

                if (teacher == null || teacher.AppRole != AppRole.Teacher)
                    // Todo: по хорошему нужно дать нормально понять пользователю, что указанный user не может быть учителем
                    return NotFound();

                group.Name = model.Name;
                group.CourseId = model.CourseId;
                group.TeacherId = model.TeacherId;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await IsGroupExistAsync(model.Id))
                    return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

        await SetCoursesForViewBagAsync(model.CourseId);
        await SetTeachersForViewBagAsync(model.TeacherId);
        return View(model);
    }

    // GET: Groups/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var group = await _context
            .Groups
            .Include(g => g.Students)
            .Include(g => g.Course)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (group == null)
            return NotFound();

        var deleteGroupModel = new DeleteGroupFormModel()
        {
            Id = group.Id,
            Name = group.Name,
            CourseName = group.Course.Name
        };

        return View(deleteGroupModel);
    }

    // POST: Groups/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var group = await _context.Groups.FindAsync(id);

        // Todo: нужна проверка, что у группы нет студентов.
        // Иначе нельзя удалить
        if (group != null)
        {
            _context.Groups.Remove(@group);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> IsGroupExistAsync(int id)
    {
        return await _context.Groups.AnyAsync(e => e.Id == id);
    }

    private async Task SetCoursesForViewBagAsync(int? id = null)
    {
        var courses = await _context
            .Courses
            .Select(x => new { Id = x.Id, Name = x.Name })
            .ToListAsync();

        ViewBag.Courses = id == null 
            ? new SelectList(courses, "Id", "Name") 
            : new SelectList(courses, "Id", "Name", id);
    }

    private async Task SetTeachersForViewBagAsync(int? id = null)
    {
        var teachers = await _context
            .Users
            .Where(x => x.AppRole == AppRole.Teacher)
            .Select(x => new { Id = x.Id, Name = x.FirstName })
            .ToListAsync();

        ViewBag.Teachers = id == null 
            ? new SelectList(teachers, "Id", "Name") 
            : new SelectList(teachers, "Id", "Name", id);
    }
}