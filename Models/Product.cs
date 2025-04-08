using System.ComponentModel.DataAnnotations;

namespace Neemah.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; } // Primary Key

        [Required]
        [Display(Name = "Serial Number")]
        public string SN { get; set; }

        [Required]
        [Display(Name = "Item Name (Arabic)")]
        public string ItemNameArabic { get; set; }

        [Required]
        [Display(Name = "Item Name (English)")]
        public string ItemNameEnglish { get; set; }

        [Required]
        public string Unit { get; set; }

        [Required]
        public string? ImagePath { get; set; }

        [Display(Name = "Raw Materials (g)")]
        [Range(0, double.MaxValue)]
        public double RawMaterials { get; set; }

        [Display(Name = "Additions (g)")]
        [Range(0, double.MaxValue)]
        public double Additions { get; set; }

        [Display(Name = "Spices Weight (g)")]
        [Range(0, double.MaxValue)]
        public double SpicesWeight { get; set; }

        [Display(Name = "Salt Weight (g)")]
        [Range(0, double.MaxValue)]
        public double SaltWeight { get; set; }

        [Display(Name = "Plate Weight (g)")]
        [Range(0, double.MaxValue)]
        public double PlateWeight { get; set; }
    }
}
