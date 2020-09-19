using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Retail_Management_System.Models
{
    public class Carddetails
    {
       public string CardType { get; set; }
       public string CardName { get; set; }
       public long cardnumber { get; set; }
       public int cvv { get; set; }
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