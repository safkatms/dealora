using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dealora.Models
{
    public enum ShippingStatus
    {
        Pending,
        Shipped,
        Delivered,
        Returned
    }

    public class Shipping
    {
        public int Id { get; set; }

        [ForeignKey("Order")]
        [Required(ErrorMessage = "Order ID is required")]
        public int OrderId { get; set; }  // Foreign key for Order

        [Required(ErrorMessage = "Shipping Method is required")]
        [StringLength(100, ErrorMessage = "Shipping Method cannot exceed 100 characters")]
        public string ShippingMethod { get; set; }

        [StringLength(100, ErrorMessage = "Tracking Number cannot exceed 100 characters")]
        public string TrackingNumber { get; set; }

        [Required(ErrorMessage = "Shipping Status is required")]
        public ShippingStatus ShippingStatus { get; set; }

        public DateTime? ShippedDate { get; set; }  // Nullable for when shipping hasn't occurred yet

        // Navigation property
        public virtual Order Order { get; set; }

        // Default constructor
        public Shipping()
        {
            ShippingStatus = ShippingStatus.Pending;  // Default to Pending when created
        }
    }
}
