using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationMVC.Models.Database;
using WebApplicationMVC.ViewModels;

namespace WebApplicationMVC.Controllers;

[Authorize("admin")]
public class CoursesController : Controller
{
    private readonly AppDbContext _context;

    public CoursesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Courses
    public async Task<IActionResult> Index()
    {
        var courses = await _context.Courses.ToListAsync();

        var courseViewModels = courses
            .Select(c => new CourseViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            })
            .ToList();
        return View(courseViewModels);
    }

    // GET: Courses/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var course = await _context.Courses
            .FirstOrDefaultAsync(m => m.Id == id);
        if (course == null)
        {
            return NotFound();
        }

        var courseViewModel = new CourseViewModel()
        {
            Id = course.Id,
            Name = course.Name,
            Description = course.Description
        };

        return View(courseViewModel);
    }

    // GET: Courses/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Courses/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description")] CreateCourseFormModel model)
    {
        if (ModelState.IsValid)
        {
            var course = new Course()
            {
                Name = model.Name,
                Description = model.Description
            };

            _context.Add(course);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        
        return View(model);
    }


    // GET: Courses/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var course = await _context.Courses.FindAsync(id);
        if (course == null)
        {
            return NotFound();
        }

        var editCourseFormModel = new EditCourseFormModel()
        {
            Id = course.Id,
            Name = course.Name,
            Description = course.Description
        };
        
        return View(editCourseFormModel);
    }

    // POST: Courses/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] EditCourseFormModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == model.Id);

                if (course == null)
                    return NotFound();

                course.Name = model.Name;
                course.Description = model.Description;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(model.Id))
                {
                    return NotFound();
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    // GET: Courses/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var course = await _context.Courses
            .FirstOrDefaultAsync(m => m.Id == id);

        if (course == null)
        {
            return NotFound();
        }

        var deleteCourseModel = new DeleteCourseFormModel()
        {
            Id = course.Id,
            Name = course.Name,
            Description = course.Description,
        };

        return View(deleteCourseModel);
    }

    // POST: Courses/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, DeleteCourseFormModel model)
    {
        var course = await _context.Courses.FindAsync(id);

        if (course != null)
        {
            // Todo: необходимо проверять есть ли у кура группы. Если есть, то запретить удаление
            _context.Courses.Remove(course);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool CourseExists(int id)
    {
        return _context.Courses.Any(e => e.Id == id);
    }
}
