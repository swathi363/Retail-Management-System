using Retail_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Retail_Management_System.Controllers
{
    public class ProductController : Controller
    {
        RetailContext db = new RetailContext();
        // GET: Product
        public ActionResult Index(string category)
        {
            if (category == null)
            {
                var cat = db.Products
                                  .Select(p => p.CategoryName)
                                  .Distinct();
                ViewBag.category = cat.ToList();
                // returns the category view(Index view)
                //var cat = db.Products.GroupBy(m=>m.CategoryName).Select(y=>y.Count());
                return View();
            }
            else
            {
                if (category == "All")
                {
                    // return the View named Products with all products list
                    return View("Index", db.Products.ToList());
                }
                else
                {
                    var products = db.Products.Where(p => p.CategoryName.Equals(category)).ToList();

                    if (products != null)
                    {
                        // return the View named Products with the required category lists 
                        return View("Products", products);
                    }
                    else
                    {
                        ViewBag.Error = "Invalid Category";
                    }
                    // return the View named Products without any data 
                    return View("Products");
                }
            }
        }
        public ActionResult ProductView(string ProductId)
        {
            if (ProductId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(ProductId);
            if (product == null)
            {
                return HttpNotFound();
            }
            
            if (product.Stock<30)
            {
                product.SpecialDiscount = 25;

            }
            DateTime dt1 = new DateTime(2020, 09, 23);
            DateTime dt2 = new DateTime(2020, 09, 18);
            DateTime dt3 = new DateTime(2020, 09, 22);
            if (DateTime.Now == dt1)
            {
                product.SpecialDiscount = 24;
            }
            if (DateTime.Now == dt2)
            {
                product.SpecialDiscount = 23;
            }
            if (DateTime.Now == dt3)
            {
                product.SpecialDiscount = 13;
            }
            return View();
        }
    }
}