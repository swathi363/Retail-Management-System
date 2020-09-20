using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Retail_Management_System.Models
{
    public class Cart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartId { get; set; }
        [DataType(DataType.EmailAddress)]
        public string UserId { get; set; }//Also a foreign key from user table
        public string ProductId { get; set; }//Also a foregin key from product table
        public string ProductName { get; set; }
        [Display(Name ="Quantity")]
        public int NoofProduct { get; set; }
        public double Amount { get; set; }

        public virtual User users { get; set; }
        public virtual Product product { get; set; }
    }
}