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
        var groups = await _context
            .Groups
            .Include(g => g.Course)
            .Include(g => g.Teacher)
            .ToListAsync();

        var groupsViewModels = groups.Select(g => new GroupViewModel()
        {
            Id = g.Id,
            Name = g.Name,
            CourseName = g.Course.Name,
            TeacherName = g.Teacher.Name,
        });

        return View(groupsViewModels);
    }

    // GET: Groups/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var group = await _context
            .Groups
            .Include(g => g.Course)
            .Include(g => g.Teacher)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (group == null)
        {
            return NotFound();
        }

        var gropViewModel = new GroupViewModel()
        {
            Id = group.Id,
            Name = group.Name,
            CourseName = group.Course.Name,
            TeacherName = group.Teacher.Name,
        };

        return View(gropViewModel);
    }

    // GET: Groups/Create
    public async Task<IActionResult> Create()
    {
        ViewData["Courses"] = await _context
            .Courses
            .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
            .ToListAsync();

        ViewData["Teachers"] = await _context
            .Users
            .Where(x => x.AppRole == AppRole.Teacher)
            .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
            .ToListAsync();

        return View();
    }

    // POST: Groups/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Name,CourseId,TeacherId")] CreateGroupFormModel model)
    {
        if (ModelState.IsValid)
        {
            var teacher = await _context
                .Users
                .FirstOrDefaultAsync(x => x.Id == model.TeacherId && x.AppRole == AppRole.Teacher);

            if (teacher == null || teacher.Name == "Мария Teacher" || teacher.AppRole != AppRole.Teacher)
            {
                ModelState.AddModelError("TeacherId", "Ошибка при выборе преподователя");
                // Todo: по хорошему нужно дать нормально понять пользователю, что указанный user не может быть учителем
                ViewData["Courses"] = await _context
                    .Courses
                    .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
                    .ToListAsync();

                ViewData["Teachers"] = await _context
                    .Users
                    .Where(x => x.AppRole == AppRole.Teacher)
                    .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
                    .ToListAsync();
                return View(model);
            }


            var course = await _context
                .Courses
                .FirstOrDefaultAsync(x => x.Id == model.CourseId);

            if (course == null)
                // Todo: по хорошему нужно дать нормально понять пользователю
                return RedirectToAction(nameof(Create));

            // Todo: нужно проверить, что такое CourseId
            var group = new Group()
            {
                Name = model.Name,
                //CourseId = model.CourseId,
                Course = course,
                Teacher = teacher,
                //TeacherId = model.TeacherId,
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
        {
            return NotFound();
        }

        var group = await _context.Groups.FindAsync(id);
        if (group == null)
        {
            return NotFound();
        }

        var groupEditModel = new EditGroupFormModel()
        {
            Id = group.Id,
            Name = group.Name,
            CourseId = group.CourseId,
            TeacherId = group.TeacherId,
        };

        var courses = await _context
            .Courses
            .Select(x => new { Id = x.Id, Name = x.Name })
            .ToListAsync();

        ViewBag.Courses = new SelectList(courses, "Id", "Name", group.CourseId);

        //ViewBag.Courses = coursesItemList;

        var teachers = await _context
            .Users
            .Where(x => x.AppRole == AppRole.Teacher)
            .Select(x => new { Id = x.Id, Name = x.Name })
            .ToListAsync();

        ViewBag.Teachers = new SelectList(teachers, "Id", "Name", group.TeacherId);

        return View(groupEditModel);
    }

    // POST: Groups/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id, [Bind("Id,Name,CourseId,TeacherId")] EditGroupFormModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

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
                if (!GroupExists(model.Id))
                {
                    return NotFound();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        var courses = await _context
            .Courses
            .Select(x => new { Id = x.Id, Name = x.Name })
            .ToListAsync();

        var coursesItemList = new SelectList(courses, "Id", "Name", model.CourseId);

        ViewBag.Courses = coursesItemList;

        return View(model);
    }

    // GET: Groups/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var group = await _context
            .Groups
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

    private bool GroupExists(int id)
    {
        return _context.Groups.Any(e => e.Id == id);
    }

    private SelectList GetCoursesForBag()
    {

    } 
}
