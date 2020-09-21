using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;

namespace Retail_Management_System.Models
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name ="Transaction Id")]
        public int Tid { get; set; }
        [Display(Name = "Bill No")]

        public int BillNo { get; set; }
        [DataType(DataType.EmailAddress)]
        public string UserId { get; set; }
        public string ProductId { get; set; }
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }
        [Display(Name = "No of Product")]

        public int NoofProduct { get; set; }
        public double Amount { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Transactin Date")]

        public DateTime Tdate { get; set; }
        public virtual User user { get; set; }
        public virtual Product product { get; set; }
        public virtual Bill Bill { get; set; }
    }

    
}