using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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


        public string FirstName { get; set; }


        public string LastName { get; set; }


        public string Email { get; set; }


        public string Password { get; set; }


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
