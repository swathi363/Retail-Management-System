using Retail_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
                    return View("Products", db.Products.ToList());
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
        //Get Search product by productname
        
        public ActionResult Search(string ProductName)
        {
            var list = new List<Product>();
            if (String.IsNullOrEmpty(ProductName))
            {
                list = db.Products.ToList();
            }
            else
            {
                list = db.Products.Where(p => p.ProductName.ToLower().Equals(ProductName.ToLower())).ToList();
                if (list.Count == 0)
                {
                    list = db.Products.Where(p => p.CategoryName.ToLower().Equals(ProductName.ToLower())).ToList();
                    if (list.Count == 0)
                    {
                        list = db.Products.Where(p => p.BrandName.ToLower().Equals(ProductName.ToLower())).ToList();
                    }
                }
            }
            ViewBag.Title = "Search Result";
            return View("Products", list);
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
                db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }
            DateTime dt1 = new DateTime(2020, 09, 23);
            DateTime dt2 = new DateTime(2020, 09, 18);
            DateTime dt3 = new DateTime(2020, 09, 22);
            if (DateTime.Now == dt1)
            {
                product.SpecialDiscount = 24;
                db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();


            }
            if (DateTime.Now == dt2)
            {
                product.SpecialDiscount = 23;
                db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }
            if (DateTime.Now == dt3)
            {
                product.SpecialDiscount = 13;
                db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }
            return View(product);
        }
        [Authorize]
        public ActionResult AddtoCart(string itemno, string ProductId)
        {
            string UserId = Session["UserId"].ToString();
            int noofunits = int.Parse(itemno);
            if(Session["UserId"]==null)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                if (String.IsNullOrEmpty(ProductId))
                {
                    ViewBag.Error = "Empty";
                }
                else
                {
                    var p = db.Products.Where(pro => pro.Productid.Equals(ProductId)).FirstOrDefault();

                    if (noofunits > p.Stock)
                    {
                        ViewBag.Error = "No stock available";
                    }
                    else
                    {
                        if (db.Carts.Where(car => car.ProductId.Equals(ProductId) && car.UserId.Equals(UserId)).FirstOrDefault() != null)
                        {

                            Cart cart = db.Carts.Where(car => car.ProductId.Equals(ProductId) && car.UserId.Equals(UserId)).FirstOrDefault();
                            cart.NoofProduct = cart.NoofProduct + noofunits;
                            db.Entry(cart).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            Cart cart = new Cart();
                            cart.UserId = Session["UserId"].ToString();
                            cart.ProductId = p.Productid;
                            cart.ProductName = p.ProductName;
                            cart.NoofProduct = noofunits;
                            cart.Amount = p.GetAmount(p.Price, p.Discount,p.SpecialDiscount, noofunits);
                            db.Carts.Add(cart);
                            db.SaveChanges();
                        }
                    }
                }
            }
            return RedirectToAction("Cart", "User");
        }
        }
    }
