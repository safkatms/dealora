using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [ForeignKey("User")]
        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }  // Foreign key for User (Customer)

        [Required(ErrorMessage = "Order Date is required")]
        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage = "Total Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total Amount must be greater than 0")]
        public double TotalAmount { get; set; }

        [Required(ErrorMessage = "Order Status is required")]
        public OrderStatus Status { get; set; }

        [ForeignKey("Address")]
        [Required(ErrorMessage = "Address ID is required")]
        public int AddressId { get; set; }  // Foreign key for Address

        [Required(ErrorMessage = "Payment Method is required")]
        public PaymentMethod PaymentMethod { get; set; }

        public virtual User User { get; set; }
        public virtual Address Address { get; set; }

        // Default constructor
        public Order()
        {
            OrderDate = DateTime.Now;
            Status = OrderStatus.Pending;  // Default to Pending when the order is created
        }
    }
}
