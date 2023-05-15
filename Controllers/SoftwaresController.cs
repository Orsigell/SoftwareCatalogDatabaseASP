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
    public class SoftwaresController : Controller
    {
        private readonly SoftwareCatalogDBContext _context;
        private readonly IWebHostEnvironment _appEnvironment;


        public SoftwaresController(SoftwareCatalogDBContext context, IWebHostEnvironment
appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        // GET: Softwares
        public async Task<IActionResult> Index()
        {
              return _context.Softwares != null ? 
                          View(await _context.Softwares.ToListAsync()) :
                          Problem("Entity set 'SoftwareCatalogDBContext.Softwares'  is null.");
        }

        // GET: Softwares/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Softwares == null)
            {
                return NotFound();
            }

            var software = await _context.Softwares.Include(s => s.Screens).Include(s => s.Categories).Include(s => s.Comments)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (software == null)
            {
                return NotFound();
            }
            List<byte[]> screensList = new List<byte[]>();
            foreach (var item in software.Screens)
            {
                screensList.Add(System.IO.File.ReadAllBytes(_appEnvironment.WebRootPath + item.Screen));
            }
            ViewBag.screens = screensList;
            if (!(String.IsNullOrEmpty(software.Image)))
            {
                byte[] imageData = System.IO.File.ReadAllBytes(_appEnvironment.WebRootPath + software.Image);
                ViewBag.imageData = imageData;
            }
            else
            {
                ViewBag.imageData = null;
            }
            return View(software);
        }

        // GET: Softwares/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Softwares/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Discription,Image,Link,SystemRequirements,LicensName,LicenseType,LicensePrice,LicenseDuration")] Software software, IFormFile upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null)
                {
                    string path = "/Files/" + upload.FileName;
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await upload.CopyToAsync(fileStream);
                    }
                    software.Image = path;
                }
                _context.Add(software);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(software);
        }
        // GET: Softwares/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Softwares == null)
            {
                return NotFound();
            }

            var software = await _context.Softwares.FindAsync(id);
            if (software == null)
            {
                return NotFound();
            }
            if (!(String.IsNullOrEmpty(software.Image)))
            {
                byte[] imageData = System.IO.File.ReadAllBytes(_appEnvironment.WebRootPath + software.Image);
                ViewBag.imageData = imageData;
            }
            else
            {
                ViewBag.imageData = null;
            }
            return View(software);
        }

        // POST: Softwares/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Discription,Image,Link,SystemRequirements,LicensName,LicenseType,LicensePrice,LicenseDuration")] Software software)
        {
            if (id != software.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(software);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SoftwareExists(software.Id))
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
            if (!(String.IsNullOrEmpty(software.Image)))
            {
                byte[] imageData = System.IO.File.ReadAllBytes(_appEnvironment.WebRootPath + software.Image);
                ViewBag.imageData = imageData;
            }
            else
            {
                ViewBag.imageData = null;
            }
            return View(software);
        }

        // GET: Softwares/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Softwares == null)
            {
                return NotFound();
            }

            var software = await _context.Softwares
                .FirstOrDefaultAsync(m => m.Id == id);
            if (software == null)
            {
                return NotFound();
            }

            return View(software);
        }

        // POST: Softwares/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Softwares == null)
            {
                return Problem("Entity set 'SoftwareCatalogDBContext.Softwares'  is null.");
            }
            var software = await _context.Softwares.FindAsync(id);
            if (software != null)
            {
                _context.Softwares.Remove(software);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SoftwareExists(int id)
        {
          return (_context.Softwares?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
