using System.ComponentModel.DataAnnotations;

namespace SoftwareCatalogDatabaseASP.Models
{
    public class Comments
    {
        [Display(Name = "Автор комментария")]
        public string Name { get; set; }
        [Display(Name = "Текст комментария")]
        public string Text { get; set; }
        [Display(Name = "Программа")]
        public int SoftwareId { get; set; }
        public Software? Software { get; set; }
    }
}