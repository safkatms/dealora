using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Dealora.Models.ViewModel
{
    public class CheckoutViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public List<Address> Addresses { get; set; }

        [Required(ErrorMessage = "Please select an address.")]
        public int AddressId { get; set; }

        public string DiscountCode { get; set; }

        [Required(ErrorMessage = "Payment method is required.")]
        public PaymentMethod PaymentMethod { get; set; }

        public double TotalAmount { get; set; }
    }
}