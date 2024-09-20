using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dealora.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }  // Foreign key for Category
        public int SellerId { get; set; }    // Foreign key for User (Seller)
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsActive { get; set; }
        public  virtual Category Category { get; set; }
        public virtual User User { get; set; }
        public Product()
        {
            DateAdded = DateTime.Now;
            IsActive = true;
        }
    }
}