using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dealora.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }  // Foreign key for User (Customer)

        [Required(ErrorMessage = "Date Created is required")]
        public DateTime DateCreated { get; set; }

        // Navigation property
        public virtual User User { get; set; }

        // Default constructor
        public ShoppingCart()
        {
            DateCreated = DateTime.Now;
        }
    }
}
