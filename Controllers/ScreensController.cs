using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoftwareCatalogDatabaseASP.Models;

namespace SoftwareCatalogDatabaseASP.Controllers
{
    public class ScreensController : Controller
    {
        private readonly SoftwareCatalogDBContext _context;

        public ScreensController(SoftwareCatalogDBContext context)
        {
            _context = context;
        }

        // GET: Screens
        public async Task<IActionResult> Index()
        {
            var softwareCatalogDBContext = _context.Screens.Include(s => s.Software);
            return View(await softwareCatalogDBContext.ToListAsync());
        }

        // GET: Screens/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Screens == null)
            {
                return NotFound();
            }

            var screens = await _context.Screens
                .Include(s => s.Software)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (screens == null)
            {
                return NotFound();
            }

            return View(screens);
        }

        // GET: Screens/Create
        public IActionResult Create()
        {
            ViewData["SoftwareId"] = new SelectList(_context.Softwares, "Id", "Id");
            return View();
        }

        // POST: Screens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Screen,SoftwareId")] Screens screens)
        {
            if (ModelState.IsValid)
            {
                _context.Add(screens);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SoftwareId"] = new SelectList(_context.Softwares, "Id", "Id", screens.SoftwareId);
            return View(screens);
        }

        // GET: Screens/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Screens == null)
            {
                return NotFound();
            }

            var screens = await _context.Screens.FindAsync(id);
            if (screens == null)
            {
                return NotFound();
            }
            ViewData["SoftwareId"] = new SelectList(_context.Softwares, "Id", "Id", screens.SoftwareId);
            return View(screens);
        }

        // POST: Screens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Screen,SoftwareId")] Screens screens)
        {
            if (id != screens.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(screens);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScreensExists(screens.Id))
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
            ViewData["SoftwareId"] = new SelectList(_context.Softwares, "Id", "Id", screens.SoftwareId);
            return View(screens);
        }

        // GET: Screens/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Screens == null)
            {
                return NotFound();
            }

            var screens = await _context.Screens
                .Include(s => s.Software)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (screens == null)
            {
                return NotFound();
            }

            return View(screens);
        }

        // POST: Screens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Screens == null)
            {
                return Problem("Entity set 'SoftwareCatalogDBContext.Screens'  is null.");
            }
            var screens = await _context.Screens.FindAsync(id);
            if (screens != null)
            {
                _context.Screens.Remove(screens);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScreensExists(int id)
        {
          return (_context.Screens?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
