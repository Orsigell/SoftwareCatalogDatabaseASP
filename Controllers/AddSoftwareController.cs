using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftwareCatalogDatabaseASP.Models;

namespace SoftwareCatalogDatabaseASP.Controllers
{
    public class AddSoftwareController : Controller
    {
        private readonly SoftwareCatalogDBContext _context;

        public AddSoftwareController(SoftwareCatalogDBContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Softwares.ToListAsync());
		}
		[HttpPost]
		public async Task<IActionResult> Index(int softwareId)
		{
			Software software = await _context.Softwares.FindAsync(softwareId);
			if (software == null)
				return NotFound();

			ViewBag.SoftwareId = softwareId;
			ViewBag.Software = software;
			return View("ChooseCategory", await _context.Categories.ToListAsync());
		}
	}
}
