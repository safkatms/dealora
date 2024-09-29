using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dealora.Models.ViewModel
{
    public class OrderHistoryViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalAmount { get; set; }
        public string Status { get; set; }
        public List<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();
    }

    public class OrderItemViewModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double PriceAtPurchase { get; set; }
    }

    public class SellerOrderViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double PriceAtPurchase { get; set; }
        public double TotalAmount { get; set; } // Total amount for this item
        public string CurrentStatus { get; set; } // Current status of the order item
        public string NewStatus { get; set; } // New status selected by the seller
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhoneNumber { get; set; }
    }


}