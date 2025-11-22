using System.ComponentModel.DataAnnotations;

namespace AgriEnergy.Models
{
    public class Product
    {
        public int Id { get; set; }


       

        [Display(Name = "Farmer Name")]

        public string? FarmerName { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Category is required.")]

        [Display(Name = "Product Category")]
        public string category { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Date is required.")]

        [Display(Name = "Date Added")]
        public DateTime DateTime { get; set; }
    }
}
