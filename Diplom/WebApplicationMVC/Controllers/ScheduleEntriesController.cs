using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplicationMVC.Models.Database;

namespace WebApplicationMVC.Controllers
{
    public class ScheduleEntriesController : Controller
    {
        private readonly AppDbContext _context;

        public ScheduleEntriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ScheduleEntries
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.ScheduleEntries.Include(s => s.Group);
            return View(await appDbContext.ToListAsync());
        }

        // GET: ScheduleEntries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheduleEntry = await _context.ScheduleEntries
                .Include(s => s.Group)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (scheduleEntry == null)
            {
                return NotFound();
            }

            return View(scheduleEntry);
        }

        // GET: ScheduleEntries/Create
        public IActionResult Create()
        {
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Id");
            return View();
        }

        // POST: ScheduleEntries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,GroupId,Start,End")] ScheduleEntry scheduleEntry)
        {
            if (ModelState.IsValid)
            {
                _context.Add(scheduleEntry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Id", scheduleEntry.GroupId);
            return View(scheduleEntry);
        }

        // GET: ScheduleEntries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheduleEntry = await _context.ScheduleEntries.FindAsync(id);
            if (scheduleEntry == null)
            {
                return NotFound();
            }
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Id", scheduleEntry.GroupId);
            return View(scheduleEntry);
        }

        // POST: ScheduleEntries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GroupId,Start,End")] ScheduleEntry scheduleEntry)
        {
            if (id != scheduleEntry.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(scheduleEntry);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduleEntryExists(scheduleEntry.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Id", scheduleEntry.GroupId);
            return View(scheduleEntry);
        }

        // GET: ScheduleEntries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheduleEntry = await _context.ScheduleEntries
                .Include(s => s.Group)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (scheduleEntry == null)
            {
                return NotFound();
            }

            return View(scheduleEntry);
        }

        // POST: ScheduleEntries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var scheduleEntry = await _context.ScheduleEntries.FindAsync(id);
            if (scheduleEntry != null)
            {
                _context.ScheduleEntries.Remove(scheduleEntry);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScheduleEntryExists(int id)
        {
            return _context.ScheduleEntries.Any(e => e.Id == id);
        }
    }
}
