using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dealora.Models.ViewModel
{
    public class CategoryViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
        public Category NewCategory { get; set; }
    }
}