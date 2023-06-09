﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using SoftwareCatalogDatabaseASP.Models;

namespace SoftwareCatalogDatabaseASP.Controllers
{
    public class CommentsController : Controller
    {
        private readonly SoftwareCatalogDBContext _context;
        private readonly IWebHostEnvironment _appEnvironment;

        public CommentsController(SoftwareCatalogDBContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        // GET: Comments
        [Authorize(Roles = "admin, coach")]
        public async Task<IActionResult> Index()
        {
            var softwareCatalogDBContext = _context.Comments.Include(c => c.Software);
            return View(await softwareCatalogDBContext.ToListAsync());
        }

        // GET: Comments/Details/5
        [Authorize(Roles = "admin, coach")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comments = await _context.Comments
                .Include(c => c.Software)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comments == null)
            {
                return NotFound();
            }

            return View(comments);
        }

        // GET: Comments/Create
        [Authorize(Roles = "admin, coach")]
        public IActionResult Create()
        {
            ViewData["SoftwareId"] = new SelectList(_context.Softwares, "Id", "Name");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, coach")]
        public async Task<IActionResult> Create([Bind("Id,Name,Text,SoftwareId")] Comments comments)
        {
            if (ModelState.IsValid)
            {
                _context.Add(comments);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SoftwareId"] = new SelectList(_context.Softwares, "Id", "Id", comments.SoftwareId);
            return View(comments);
        }

        // GET: Comments/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comments = await _context.Comments.FindAsync(id);
            if (comments == null)
            {
                return NotFound();
            }
            ViewData["SoftwareId"] = new SelectList(_context.Softwares, "Id", "Name", comments.SoftwareId);
            return View(comments);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Text,SoftwareId")] Comments comments)
        {
            if (id != comments.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comments);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentsExists(comments.Id))
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
            ViewData["SoftwareId"] = new SelectList(_context.Softwares, "Id", "Id", comments.SoftwareId);
            return View(comments);
        }

        // GET: Comments/Delete/5
        [Authorize(Roles = "admin, coach")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comments = await _context.Comments
                .Include(c => c.Software)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comments == null)
            {
                return NotFound();
            }

            return View(comments);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, coach")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Comments == null)
            {
                return Problem("Entity set 'SoftwareCatalogDBContext.Comments'  is null.");
            }
            var comments = await _context.Comments.FindAsync(id);
            if (comments != null)
            {
                _context.Comments.Remove(comments);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentsExists(int id)
        {
          return (_context.Comments?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        [Authorize(Roles = "admin, coach")]
        public FileResult GetReport()
        {
            string path = "/Reports/comments_report_template.xlsx";
            string result = "/Reports/comments_report.xlsx";
            FileInfo fi = new FileInfo(_appEnvironment.WebRootPath + path);
            FileInfo fr = new FileInfo(_appEnvironment.WebRootPath + result);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                excelPackage.Workbook.Properties.Author = "Вертоградов И.А.";
                excelPackage.Workbook.Properties.Title = "Отчёт по комментариям";
                excelPackage.Workbook.Properties.Subject = "Комментарии";
                excelPackage.Workbook.Properties.Created = DateTime.Now;
                //плучаем лист по имени.
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Comments"];
                //получаем списко пользователей и в цикле заполняем лист данными
                int startLine = 3;
                List<Comments> comments = _context.Comments.Include(s => s.Software).ToList();
                foreach (Comments coment in comments)
                {
                    worksheet.Cells[startLine, 1].Value = startLine - 2;
                    worksheet.Cells[startLine, 2].Value = coment.Id;
                    worksheet.Cells[startLine, 3].Value = coment.Text;
                    if (coment.Software!=null)
                    {
                        worksheet.Cells[startLine, 4].Value = coment.Software.Name;
                    }
                    startLine++;
                }
                //созраняем в новое место
                excelPackage.SaveAs(fr);
            }
            // Тип файла - content-type
            string file_type =
           "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet";
            // Имя файла - необязательно
            string file_name = "comments_report.xlsx";
            return File(result, file_type, file_name);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, coach, guest")]
        public async Task<IActionResult> SendComment(string Author, string CommentText, int Id)
        {
            TempData["Author"] = Author;
            TempData["CommentText"] = CommentText;

            if ((Author != null) || (CommentText != null))
            {
                Comments comments = new Comments()
                {
                    Name = Author,
                    Text = CommentText,
                    SoftwareId = Id
                };
                _context.Add(comments);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "Softwares", new { id = Id });
        }
    }
}
