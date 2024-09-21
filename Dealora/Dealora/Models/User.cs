using System;
using System.ComponentModel.DataAnnotations;

namespace Dealora.Models
{
    public enum UserRole
    {
        Admin,
        Customer,
        Seller
    }

    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, ErrorMessage = "First Name cannot be longer than 50 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, ErrorMessage = "Last Name cannot be longer than 50 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        public string Password { get; set; }

        [Phone(ErrorMessage = "Invalid Phone Number")]
        [StringLength(20, ErrorMessage = "Phone Number cannot be longer than 20 characters")]
        public string PhoneNumber { get; set; }

        public UserRole Role { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsActive { get; set; }

        public User()
        {
            Role = UserRole.Customer;
            DateCreated = DateTime.Now;
            IsActive = true;
        }
    }
}
