using System;
using System.ComponentModel.DataAnnotations;

namespace Neemah.Models
{
    public class FactoryOrder
    {
        [Key] // Marking Id as the primary key
        public int Id { get; set; }

        [Display(Name = "Product")]
        [Required(ErrorMessage = "Please select a product.")]
        public int ProductId { get; set; }  // Foreign key to Product table
        public Product Product { get; set; }  // Navigation property to the Product

        [Display(Name = "Order Date")]
        [Required(ErrorMessage = "Please select the order date.")]
        public DateTime Date { get; set; }  // Date of order

        [Display(Name = "Order Quantity")]
        [Required(ErrorMessage = "Please enter the order quantity.")]
        [Range(1, int.MaxValue, ErrorMessage = "Order quantity must be greater than 0.")]
        public int Order { get; set; }  // Order quantity

        [Display(Name = "Stock Quantity")]
        [Required(ErrorMessage = "Please enter the stock quantity.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be a non-negative number.")]
        public int Stock { get; set; }  // Stock quantity

        [Display(Name = "Branch")]
        [Required(ErrorMessage = "Please enter the branch name.")]
        public string Branch { get; set; }  // Branch name
    }
}
