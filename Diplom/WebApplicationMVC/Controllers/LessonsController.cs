using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplicationMVC.Models.Database;
using WebApplicationMVC.ViewModels.Lessons;

namespace WebApplicationMVC.Controllers;

[Authorize("admin")]
public class LessonsController : Controller
{
    private readonly AppDbContext _context;

    public LessonsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Lessons
    public async Task<IActionResult> Index()
    {
        var lessons = await _context
            .Lessons
            .Include(l => l.Group)
            .ThenInclude(g => g.Teacher)
            .Include(l => l.Group)
            .ThenInclude(g => g.Students)
            .Select(l => new LessonViewModel()
            {
                Id = l.Id,
                GroupId = l.GroupId,
                GroupName = l.Group.Name,
                TeacherName = l.Group.Teacher.LastName,
                Start = l.Start
            })
            .ToArrayAsync();

        return View(lessons);
    }

    // GET: Lessons/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var lesson = await _context
            .Lessons
            .Include(l => l.Group)
            .ThenInclude(g => g.Teacher)
            .Include(l => l.Group)
            .ThenInclude(g => g.Students)
            .Select(l => new LessonDetailsViewModel()
            {
                Id = l.Id,
                GroupId = l.GroupId,
                GroupName = l.Group.Name,
                TeacherName = l.Group.Teacher.LastName,
                Start = l.Start
            })
            .FirstOrDefaultAsync();

        if (lesson == null)
            return NotFound();

        return View(lesson);
    }

    // GET: Lessons/Create
    public async Task<IActionResult> Create()
    {
        await SetGroupsForViewBagAsync();
        return View();
    }

    // POST: Lessons/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateLessonFormModel model)
    {
        if (ModelState.IsValid)
        {
            var group = await _context
                .Groups
                .Include(g => g.Students)
                .FirstOrDefaultAsync(x => x.Id == model.GroupId);

            if (group == null)
            {
                ModelState.AddModelError("GroupId", "Ошибка при выборе группы");
                await SetGroupsForViewBagAsync(model.GroupId);
                return View(model);
            }

            var isAnyCrossingLessons = await _context
                .Lessons
                .Include(l => l.Group)
                .Where(l => l.Group.TeacherId == group.TeacherId &&
                    (l.Start <= model.Start.AddMinutes(60) && l.Start.AddMinutes(60) >= model.Start.AddMinutes(60) ||
                    l.Start <= model.Start && l.Start.AddMinutes(60) >= model.Start))
                .AnyAsync();

            if (isAnyCrossingLessons)
            {
                ModelState.AddModelError("Start", "Ошибка при выборе времени начала занятия. " +
                    "Занятие пересекается");
                await SetGroupsForViewBagAsync(model.GroupId);
                return View(model);
            }

            var lesson = new Lesson()
            {
                Group = group,
                Start = model.Start
            };

            _context.Add(lesson);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        await SetGroupsForViewBagAsync(model.GroupId);
        return View(model);
    }

    // GET: Lessons/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var lesson = await _context.Lessons.FindAsync(id);
        if (lesson == null)
            return NotFound();
        await SetGroupsForViewBagAsync(lesson.GroupId);

        var editFormModel = new EditLessonFormModel()
        {
            Id = lesson.Id,
            GroupId = lesson.GroupId,
            Start = lesson.Start
        };

        return View(editFormModel);
    }

    // POST: Lessons/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditLessonFormModel model)
    {
        if (id != model.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                var lesson = await _context
                    .Lessons
                    .FirstOrDefaultAsync(x => x.Id == model.Id);

                if (lesson == null)
                    return NotFound();

                var group = await _context
                    .Groups
                    .Include(g => g.Students)
                    .FirstOrDefaultAsync(x => x.Id == model.GroupId);

                if (group == null)
                {
                    ModelState.AddModelError("GroupId", "Ошибка при выборе группы");
                    await SetGroupsForViewBagAsync(model.GroupId);
                    return View(model);
                }

                var isAnyCrossingLessons = await _context
                    .Lessons
                    .Include(l => l.Group)
                    .Where(l => l.Group.TeacherId == group.TeacherId &&
                        (l.Start <= model.Start.AddMinutes(60) && l.Start.AddMinutes(60) >= model.Start.AddMinutes(60) ||
                        l.Start <= model.Start && l.Start.AddMinutes(60) >= model.Start))
                    .AnyAsync();

                if (isAnyCrossingLessons)
                {
                    ModelState.AddModelError("Start", "Ошибка при выборе времени начала занятия. " +
                        "Занятие пересекается");
                    await SetGroupsForViewBagAsync(model.GroupId);
                    return View(model);
                }

                lesson.Group = group;
                lesson.Start = model.Start;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await IsLessonExistAsync(model.Id))
                    return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

        await SetGroupsForViewBagAsync(model.GroupId);
        return View(model);
    }

    // GET: Lessons/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        // Todo: Должны убедиться, что занятие нельзя удалить
        // если по нему существует отчет о посешении или оно в прошлом
        if (id == null)
            return NotFound();

        var lesson = await _context.Lessons
            .Include(x => x.Group)
            .ThenInclude(g => g.Teacher)
            .Include(x => x.Group)
            .ThenInclude(g => g.Students)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (lesson == null)
            return NotFound();

        var viewModel = new DeleteLessonViewModel()
        {
            Id = lesson.Id,
            GroupId = lesson.GroupId,
            GroupName = lesson.Group.Name,
            TeacherName = lesson.Group.Teacher.LastName,
            Start = lesson.Start
        };

        return View(viewModel);
    }

    // POST: Lessons/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var lesson = await _context.Lessons.FindAsync(id);

        if (lesson != null)
        {
            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> IsLessonExistAsync(int id)
    {
        return await _context.Lessons.AnyAsync(e => e.Id == id);
    }

    private async Task SetGroupsForViewBagAsync(int? id = null)
    {
        var groups = await _context
            .Groups
            .Include(g => g.Students)
            .Select(x => new
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync();

        ViewBag.Groups = id == null
            ? new SelectList(groups, "Id", "Name")
            : new SelectList(groups, "Id", "Name", id);
    }
}