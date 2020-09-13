using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Retail_Management_System.Models
{
    public class RetailContext:DbContext
    {
        public RetailContext()
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }

    }
}