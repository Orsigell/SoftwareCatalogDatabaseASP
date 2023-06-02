using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using SoftwareCatalogDatabaseASP.Models;

namespace SoftwareCatalogDatabaseASP.Controllers
{
    public class AddSoftwareController : Controller
    {
        private readonly SoftwareCatalogDBContext _context;

        public AddSoftwareController(SoftwareCatalogDBContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }
        [Authorize(Roles = "admin, coach")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Softwares.ToListAsync());
		}
		[HttpPost]
        [Authorize(Roles = "admin, coach")]
        public async Task<IActionResult> Index(int softwareId)
        {
            Software software = await _context.Softwares.Include(s => s.Categories).FirstOrDefaultAsync(s => s.Id == softwareId);
            if (software == null)
                return NotFound();

            ViewBag.SoftwareId = softwareId;
            ViewBag.Software = software;
            return View("ChooseCategory", await _context.Categories.ToListAsync());
        }

        [HttpPost]
        [Authorize(Roles = "admin, coach")]
        public async Task<IActionResult> ChooseCategory(int softwareId, int[] selectedCategories)
        {
            Software software = await _context.Softwares.Include(s => s.Categories).FirstOrDefaultAsync(s => s.Id == softwareId);


            if (software == null)
                return NotFound();
            software.Categories.Clear();
            foreach (var item in selectedCategories)
            {
                Categories categories = await _context.Categories.FindAsync(item);
                if (categories == null)
                    return NotFound();
                if (software.Categories.FirstOrDefault(c => c.Name == categories.Name) == null)
				{
					Categories newCategorie = new Categories()
					{
						Name = categories.Name,
						Software = software,
						SoftwareId = softwareId
					};
					software.Categories.Add(newCategorie);
				}
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ChooseImages), new {softwareId = softwareId});
        }


        [HttpGet]
        [Authorize(Roles = "admin, coach")]
        public async Task<IActionResult> ChooseImages(int softwareId)
        {
            Software software = await _context.Softwares.FindAsync(softwareId);

            if (software == null)
                return NotFound();

            ViewBag.SoftwareId = softwareId;
            ViewBag.SoftwareName = software.Name;

            return View(new List<IFormFile>());
        }
        private readonly IWebHostEnvironment _appEnvironment;

        [HttpPost]
        [Authorize(Roles = "admin, coach")]
        public async Task<IActionResult> ChooseImages(int softwareId, List<IFormFile> screenshots)
        {
            Software software = await _context.Softwares.FindAsync(softwareId);

            if (software == null)
                return NotFound();

            var newScreens = new List<Screens>();
            foreach (var screenshot in screenshots)
            {
                if (screenshot.Length > 0)
                {
                    var fileName = Path.GetFileName(screenshot.FileName);
                    var filePath = Path.Combine(_appEnvironment.WebRootPath, "files", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await screenshot.CopyToAsync(stream);
                    }

                    newScreens.Add(new Screens { Screen = "/files/" + fileName, SoftwareId = softwareId });
                }
            }

            _context.Screens.AddRange(newScreens);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Softwares");
        }

    }
}
