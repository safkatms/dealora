using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Dealora.Models.ViewModel
{
    public class AddressUpdateModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name ="Street Address")]
        public string StreetAddress { get; set; }

        [Display(Name ="City")]
        [Required]
        public string City { get; set; }
    }
}