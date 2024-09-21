using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dealora.Models
{
    public class Review
    {
        public int Id { get; set; }

        [ForeignKey("Product")]
        [Required(ErrorMessage = "Product ID is required")]
        public int ProductId { get; set; }  // Foreign key for Product

        [ForeignKey("User")]
        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }  // Foreign key for User (Customer)

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }  // Rating between 1 and 5

        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        public string Comment { get; set; }

        [Required(ErrorMessage = "Date Posted is required")]
        public DateTime DatePosted { get; set; }

        // Navigation properties
        public virtual Product Product { get; set; }
        public virtual User User { get; set; }

        // Default constructor
        public Review()
        {
            DatePosted = DateTime.Now;
        }
    }
}
