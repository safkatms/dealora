using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dealora.Models
{
    public class Address
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }  // Foreign key for User (Customer)
        public string StreetAddress { get; set; }
        public string City { get; set; }

        public virtual User User { get; set; }

    }
}