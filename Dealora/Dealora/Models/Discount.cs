using System;
using System.ComponentModel.DataAnnotations;

namespace Dealora.Models
{
    public class Discount
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Discount Code is required")]
        [StringLength(20, ErrorMessage = "Discount Code cannot be longer than 20 characters")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Discount Percentage is required")]
        [Range(0, 100, ErrorMessage = "Discount Percentage must be between 0 and 100")]
        public double DiscountPercentage { get; set; }

        [Required(ErrorMessage = "Expiry Date is required")]
        public DateTime ExpiryDate { get; set; }

        public bool IsActive { get; set; }

        // Default constructor
        public Discount()
        {
            IsActive = true;  // Default to active when created
        }

        // Method to check if the discount has expired
        public bool IsExpired()
        {
            return DateTime.Now > ExpiryDate;
        }
    }
}
