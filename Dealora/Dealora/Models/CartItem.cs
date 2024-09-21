using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Dealora.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        [ForeignKey("ShoppingCart")]
        public int ShoppingCartId { get; set; }  // Foreign key for ShoppingCart

        [ForeignKey("Product")]
        public int ProductId { get; set; }  // Foreign key for Product

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        public virtual ShoppingCart ShoppingCart {  get; set; }
        public virtual Product Product { get; set; }

    }
}