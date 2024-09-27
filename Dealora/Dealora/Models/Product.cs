using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dealora.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product Name is required")]
        [StringLength(100, ErrorMessage = "Product Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public double Price { get; set; }

        [ForeignKey("Category")]
        [Required(ErrorMessage = "Category ID is required")]
        public int CategoryId { get; set; }  // Foreign key for Category

        [ForeignKey("User")]
        [Required(ErrorMessage = "User ID (Seller) is required")]
        public int UserId { get; set; }  // Foreign key for User (Seller)

        [Required(ErrorMessage = "Stock Quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock Quantity must be a non-negative number")]
        public int StockQuantity { get; set; }

        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Date Added is required")]
        [DataType(DataType.Date)]
        public DateTime DateAdded { get; set; }

        public bool IsActive { get; set; }

        // Navigation properties
        public virtual Category Category { get; set; }
        public virtual User User { get; set; }

        // Default constructor
        public Product()
        {
            DateAdded = DateTime.Now;
            IsActive = true;
        }
    }
}
