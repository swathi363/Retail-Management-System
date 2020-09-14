using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Retail_Management_System.Models
{
    public class Supplier
    {
        [Key]
        [Required]
        [Display(Name ="Supplier Id")]
        public string SupplierId { get; set; }
        [Required]
        [Display(Name ="Supplier Name")]
        public string SupplierName { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}