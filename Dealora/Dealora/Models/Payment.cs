using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dealora.Models
{
    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed
    }

    public enum PaymentMethod
    {
        CreditCard,
        PayPal,
        BankTransfer,
        CashOnDelivery
    }

    public class Payment
    {
        public int Id { get; set; }

        [ForeignKey("Order")]
        [Required(ErrorMessage = "Order ID is required")]
        public int OrderId { get; set; }  // Foreign key for Order

        [Required(ErrorMessage = "Payment Date is required")]
        public DateTime PaymentDate { get; set; }

        [Required(ErrorMessage = "Payment Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Payment Amount must be greater than 0")]
        public double PaymentAmount { get; set; }

        [Required(ErrorMessage = "Payment Method is required")]
        public PaymentMethod PaymentMethod { get; set; }

        [Required(ErrorMessage = "Payment Status is required")]
        public PaymentStatus PaymentStatus { get; set; }

        // Navigation property
        public virtual Order Order { get; set; }

        // Default constructor
        public Payment()
        {
            PaymentDate = DateTime.Now;
            PaymentStatus = PaymentStatus.Pending;  // Default to Pending when created
        }
    }
}
