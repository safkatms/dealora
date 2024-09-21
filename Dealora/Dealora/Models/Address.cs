using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dealora.Models
{
    public class Address
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }  // Foreign key for User (Customer)

        [Required(ErrorMessage = "Street Address is required")]
        [StringLength(100, ErrorMessage = "Street Address cannot be longer than 100 characters")]
        public string StreetAddress { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(50, ErrorMessage = "City cannot be longer than 50 characters")]
        public string City { get; set; }

        // Navigation property
        public virtual User User { get; set; }
    }
}
