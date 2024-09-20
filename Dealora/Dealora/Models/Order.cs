using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dealora.Models
{
    public enum OrderStatus
    {
        Pending,
        Shipped,
        Delivered,
        Canceled
    }

    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }  // Foreign key for User (Customer)
        public DateTime OrderDate { get; set; }
        public double TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public int ShippingAddressId { get; set; }  // Foreign key for Address
        public PaymentMethod PaymentMethod { get; set; }
        public virtual User User { get; set; }
        public virtual Address Address { get; set; }
        public Order()
        {
            OrderDate = DateTime.Now;
            Status = OrderStatus.Pending;  // Default to Pending when the order is created
        }
    }
}