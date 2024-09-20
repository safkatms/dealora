using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dealora.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }  // Foreign key for Product
        public int CustomerId { get; set; }  // Foreign key for User (Customer)
        public int Rating { get; set; }  // Rating between 1 and 5
        public string Comment { get; set; }
        public DateTime DatePosted { get; set; }

        public virtual Product Product { get; set; }
        public virtual User User { get; set; }


        public Review()
        {
            DatePosted = DateTime.Now;
        }
    }
}