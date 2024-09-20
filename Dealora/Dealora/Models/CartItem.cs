using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dealora.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }  // Foreign key for ShoppingCart
        public int ProductId { get; set; }  // Foreign key for Product
        public int Quantity { get; set; }

        public virtual ShoppingCart ShoppingCart {  get; set; }
        public virtual Product Product { get; set; }

    }
}