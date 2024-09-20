using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dealora.Models
{
    public class Discount
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public double DiscountPercentage { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }

        public Discount()
        {
            IsActive = true;  // Default to active when created
        }

        public bool IsExpired()
        {
            return DateTime.Now > ExpiryDate;
        }
    }
}