using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public int OrderId { get; set; }  // Foreign key for Order
        public DateTime PaymentDate { get; set; }
        public double PaymentAmount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public virtual Order Order { get; set; }
        public Payment()
        {
            PaymentDate = DateTime.Now;
            PaymentStatus = PaymentStatus.Pending;  // Default to Pending when created
        }
    }
}