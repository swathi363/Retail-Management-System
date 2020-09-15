using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Retail_Management_System.Models
{
    public class RetailContext : DbContext
    {
        public RetailContext():base("Name=Dbconfig")
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

    }
    
            


    
}