using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Retail_Management_System.Models
{
    public class Bill
    {
        [Key]
        public int BillNo { get; set; }
        public double Amount { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}