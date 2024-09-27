using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dealora.Models.ViewModel
{
    public class DiscountViewModel
    {
        public IEnumerable<Discount> Discounts { get; set; } // List of existing discounts
        public Discount NewDiscount { get; set; }            // Form for creating a new discount
    }
}