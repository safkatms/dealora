using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dealora.Models.ViewModel
{
    public class OrderDeatilsViewModel
    {
        public Order Order { get; set; }
        public IEnumerable<OrderItem> OrderItems { get; set; }
    }
}