using NuGet.Packaging;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftwareCatalogDatabaseASP.Models
{
    public class Software
    {
        public Software()
		{
			Categories = new List<Categories>();
			Screens = new List<Screens>();
			Comments = new List<Comments>();
		}
		public ICollection<Categories> Categories { get; set; }
		public ICollection<Screens> Screens { get; set; }
		public ICollection<Comments> Comments { get; set; }
		public int Id { get; set; }

        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Discription { get; set; }
        [Display(Name = "Иконка программы")]
        public string Image { get; set; }
        [Display(Name = "Ссылка на сайт")]
        public string Link { get; set; }
        [Display(Name = "Системные требования")]
        public string SystemRequirements { get; set; }

		[Display(Name = "Название лицензии")]
		public string LicensName { get; set; }

		[Display(Name = "Тип лицензии")]
		public string LicenseType { get; set; }

		[Display(Name = "Цена")]
		public int LicensePrice { get; set; }

		[Display(Name = "Длительность")]
		public int LicenseDuration { get; set; }
        [NotMapped]
        public bool? ToAddSoftware { get; set; }
    }
}
