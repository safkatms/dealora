using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dealora.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }  // Foreign key for Order
        public int ProductId { get; set; }  // Foreign key for Product
        public int Quantity { get; set; }
        public double PriceAtPurchase { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}