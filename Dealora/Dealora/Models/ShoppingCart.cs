using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dealora.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }  // Foreign key for User (Customer)
        public DateTime DateCreated { get; set; }
        public virtual User User { get; set; }
        public ShoppingCart()
        {
            DateCreated = DateTime.Now;
        }
    }
}