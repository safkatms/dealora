using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dealora.Models.ViewModel
{
    public class ShoppingCartViewModel
    {
        public IEnumerable<CartItem> CartItems { get; set; }
        public double TotalAmount { get; set; }
    }
}