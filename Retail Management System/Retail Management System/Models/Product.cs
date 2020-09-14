using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc.Routing.Constraints;

namespace Retail_Management_System.Models
{
    public class Product
    {
        [Key]
        [Required]
        [Display(Name = "Product ID")]
        public string Productid { get; set; }
        [Required]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }
        [Required]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; }
        [Required]
        [Display(Name = "Brand Name")]
        public string BrandName { get; set; }
        [Required]
        [Display(Name = "Price")]
        public double Price { get; set; }
        [Required]
        [Display(Name = "Stock Left")]
        public int Stock { get; set; }
        [Required]
        [Display(Name = "Sold Units")]
        public int SoldUnits { get; set; }
        [Required]
        [Display(Name = "Discount")]
        public double Discount { get; set; }
        [Required]
        [Display(Name = "Special Discount")]
        public double SpecialDiscount { get; set; }
        [Required]
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }
        public double GetAmount(double price, double discount, int units)
        {
            double TotalAmount = (price - (discount / 100) * price) * units;
            return TotalAmount;
        }
        public double GetSpecialAmount(double price, double specialdiscount, int units)
        {
            double TotalAmount = (price - (specialdiscount / 100) * price) * units;
            return TotalAmount;
        }

        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set }
    }
    public enum Category
    {
        Laptops,
        SmartPhones,
        Clothing,
        Kitchen_Appliances,
        Footwear,
        Furniture,
        Fashion,
        Utilities,
        Jewellery,
        Electricals,
        Electronics,
        Wearables,
        Refrigerators,
        Home,
        ToysBaby,
        Sports,
        Appliances,
        Other
    }
}

    
