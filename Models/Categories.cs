using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftwareCatalogDatabaseASP.Models
{
    public class Categories
    {
        public int Id { get; set; }
        [Display(Name = "Название категории")]
        public string Name { get; set; }
        [Display(Name = "Программа")]
        public int SoftwareId { get; set; }
        public Software? Software { get; set; }
        [NotMapped]
        public bool? ToAddSoftware { get; set; }
    }
}