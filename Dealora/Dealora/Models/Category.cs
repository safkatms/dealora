using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Dealora.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category Name is required")]
        [StringLength(100, ErrorMessage = "Category Name cannot be longer than 100 characters")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string Description { get; set; }  // Optional


    }
}