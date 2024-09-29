using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dealora.Models
{
    public enum OrderItemStatus
    {
        Pending,    // Order item is pending
        Confirmed,  // Order item has been confirmed by the seller
        Shipped,    // Order item has been shipped
        Delivered,  // Order item has been delivered to the customer
        Cancelled    // Order item has been canceled
    }

    public class OrderItem
    {
        public int Id { get; set; }

        [ForeignKey("Order")]
        [Required(ErrorMessage = "Order ID is required")]
        public int OrderId { get; set; }  // Foreign key for Order

        [ForeignKey("Product")]
        [Required(ErrorMessage = "Product ID is required")]
        public int ProductId { get; set; }  // Foreign key for Product

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Price at Purchase is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public double PriceAtPurchase { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public OrderItemStatus Status { get; set; } = OrderItemStatus.Pending; // Default status
        // Navigation properties
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}
