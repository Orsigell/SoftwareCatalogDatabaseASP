using System.ComponentModel.DataAnnotations;

namespace SoftwareCatalogDatabaseASP.Models
{
    public class Comments
    {
        [Display(Name = "Группа комментариев")]
        public int Id { get; set; }
        [Display(Name = "Текст отзыва")]
        public string Text { get; set; }
        [Display(Name = "Программа")]
        public int SoftwareId { get; set; }
        public Software? Software { get; set; }
    }
}