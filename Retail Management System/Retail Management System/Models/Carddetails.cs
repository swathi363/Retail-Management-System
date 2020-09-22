using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Retail_Management_System.Models
{
    public class Carddetails
    {
        [Required]
        [Display(Name ="Type of card")]
       public string CardType { get; set; }
        [Required]
        [Display(Name = "Name on Card")]
        public string CardName { get; set; }
        [Required]
        [Display(Name = "Card Number")]
        public long cardnumber { get; set; }
        [Required]
        [Display(Name = "cvv")]
        public int cvv { get; set; }
        [Required
            ]
       public string mobile { get; set; }
        public enum CardTypeName
        {
            Visa_Debit_Cards,
            Master_Cards,
            Rupay_Cards,
            Maestro_Debit_Cards,
            American_Express,
            Discover



        }

    }
}