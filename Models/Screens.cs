using System.ComponentModel.DataAnnotations;

namespace SoftwareCatalogDatabaseASP.Models
{
    public class Screens
    {
        public int Id { get; set; }
        [Display(Name = "Скриншот")]
        public string Screen { get; set; }
        [Display(Name = "Программа")]
        public int SoftwareId { get; set; }
        public Software? Software { get; set; }
    }
}