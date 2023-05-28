using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftwareCatalogDatabaseASP.Models;
using System.Diagnostics;

namespace SoftwareCatalogDatabaseASP.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
        private readonly SoftwareCatalogDBContext _context;
        private readonly IWebHostEnvironment _appEnvironment;
        public HomeController(SoftwareCatalogDBContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var softwares = await _context.Softwares.ToListAsync();
            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories;

            if (Request.Method == "POST")
            {
                var selectedCategories = new List<string>();
                foreach (var category in categories)
                {
                    if (Request.Form.ContainsKey(category.Name) && Request.Form[category.Name] == "on")
                    {
                        selectedCategories.Add(category.Name);
                    }
                }

                softwares = softwares.Where(s => s.Categories.Any(c => selectedCategories.Contains(c.Name))).ToList();
            }

            Dictionary<int, byte[]> screensDictionary = new Dictionary<int, byte[]>();
            foreach (Software item in softwares)
            {
                screensDictionary.Add(item.Id, System.IO.File.ReadAllBytes(_appEnvironment.WebRootPath + item.Image));
            }
            ViewBag.imageData = screensDictionary;
            return _context.Softwares != null ? View(softwares) :
                Problem("Entity set 'SoftwareCatalogDBContext.Softwares' is null.");
        }

        public IActionResult Privacy() 
		{
			return View();
		}
		[ResponseCache(Duration =0,Location = ResponseCacheLocation.None,NoStore =true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
		
	}
}
