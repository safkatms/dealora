using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public int OrderId { get; set; }  // Foreign key for Order
        public string ShippingMethod { get; set; }
        public string TrackingNumber { get; set; }
        public ShippingStatus ShippingStatus { get; set; }
        public DateTime? ShippedDate { get; set; }  // Nullable for when shipping hasn't occurred yet
        public virtual Order Order { get; set; }
        public Shipping()
        {
            ShippingStatus = ShippingStatus.Pending;  // Default to Pending when created
        }
    }
}