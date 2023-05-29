using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using SoftwareCatalogDatabaseASP.Models;

namespace SoftwareCatalogDatabaseASP.Controllers
{
    public class SoftwaresController : Controller
    {
        private readonly SoftwareCatalogDBContext _context;
        private readonly IWebHostEnvironment _appEnvironment;
        public SoftwaresController(SoftwareCatalogDBContext context, IWebHostEnvironment appEnvironment)
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

        public FileResult GetReport()
        {
            string path = "/Reports/software_report_template.xlsx";
            string result = "/Reports/software_report.xlsx";
            FileInfo fi = new FileInfo(_appEnvironment.WebRootPath + path);
            FileInfo fr = new FileInfo(_appEnvironment.WebRootPath + result);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                excelPackage.Workbook.Properties.Author = "Вертоградов И.А.";
                excelPackage.Workbook.Properties.Title = "Отчёт по программам";
                excelPackage.Workbook.Properties.Subject = "Программы";
                excelPackage.Workbook.Properties.Created = DateTime.Now;
                //плучаем лист по имени.
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Software"];
                //получаем списко пользователей и в цикле заполняем лист данными
                int startLine = 3;
                List<Software> softwares = _context.Softwares.ToList();
                foreach (Software software in softwares)
                {
                    worksheet.Cells[startLine, 1].Value = startLine - 2;
                    worksheet.Cells[startLine, 2].Value = software.Id;
                    worksheet.Cells[startLine, 3].Value = software.Categories.Count;
                    worksheet.Cells[startLine, 4].Value = software.Screens.Count;
                    worksheet.Cells[startLine, 5].Value = software.Comments.Count;
                    worksheet.Cells[startLine, 6].Value = software.Name;
                    worksheet.Cells[startLine, 7].Value = software.Discription;
                    worksheet.Cells[startLine, 8].Value = software.Image;
                    worksheet.Cells[startLine, 9].Value = software.Link;
                    worksheet.Cells[startLine, 10].Value = software.SystemRequirements;
                    worksheet.Cells[startLine, 11].Value = software.LicensName;
                    worksheet.Cells[startLine, 12].Value = software.LicenseType;
                    worksheet.Cells[startLine, 13].Value = software.LicensePrice;
                    worksheet.Cells[startLine, 14].Value = software.LicenseDuration;
                    startLine++;
                }
                //созраняем в новое место
                excelPackage.SaveAs(fr);
            }
            // Тип файла - content-type
            string file_type =
           "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet";
            // Имя файла - необязательно
            string file_name = "software_report.xlsx";
            return File(result, file_type, file_name);
        }

    }
}
