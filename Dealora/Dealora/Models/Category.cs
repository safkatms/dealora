using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dealora.Models
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }  // Optional
        public int? ParentCategoryId { get; set; }  // Nullable foreign key for subcategories
    }
}