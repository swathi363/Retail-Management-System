﻿using Retail_Management_System.Models;
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

            if (product.Stock < 30)
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
        public ActionResult BrandView(string Brand)
        {
            if (Brand == null)
            {
                var brand = db.Products
                                  .Select(p => p.BrandName)
                                  .Distinct();
                ViewBag.category = brand.ToList();
                
                return View();
            }
            else
            {
                    var products = db.Products.Where(p => p.BrandName.Equals(Brand)).ToList();

                    if (products != null)
                    {
                        // return the View named Products with the required category lists 
                        return View();
                    }
                    else
                    {
                        ViewBag.Error = "Invalid Category";
                    }
                    // return the View named Products without any data 
                    return View();
                
            }
        }
    
        public ActionResult Filter(string name)
        {
            var list = new List<Product>();
            if(string.IsNullOrEmpty(name))
            {
                list = db.Products.ToList();
            }
            else if(name== "Category")
            {
                return RedirectToAction("Index");
            }
            else if(name== "Brand")
            {
                return RedirectToAction("BrandView");
            }
            else if (name == "Age")
            {
                list = db.Products.Where(p=>p.PreferredAge.Equals(name)).ToList();
                
            }
            else if(name=="Gender")
            {
                list = db.Products.Where(p=>p.PreferredGender.Equals(name)).ToList();


            }

            else if (name == "Price(low to high)")
            {
                list = db.Products.Where(p => p.Price.Equals(name)).ToList();
                list.Sort();

            }
            else if (name == "Price(high to low)")
            {
                list = db.Products.Where(p => p.Price.Equals(name)).ToList();
                list.Sort();
                list.Reverse();

            }
            return View ("Products", list);


        }
        [Authorize]
        public ActionResult AddtoCart(string itemno, string ProductId)
        {
            string UserId = Session["UserId"].ToString();
            int noofunits = int.Parse(itemno);
            if (Session["UserId"] == null)
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
                            cart.Amount = p.GetAmount(p.Price, p.Discount, p.SpecialDiscount, noofunits);
                            db.Carts.Add(cart);
                            db.SaveChanges();
                        }
                    }
                }
            }
            return RedirectToAction("Cart", "User");
        }
        [Authorize]
        public ActionResult BuyNow(string UserId, string ProductId)
        {
            ViewBag.UserId = UserId;
            ViewBag.ProductId = ProductId;
            var user = db.Users.Where(u => u.UserId.Equals(UserId)).FirstOrDefault();
            return View(user);
        }
        
      
        
        
        [Authorize]
        public ActionResult BuyAll()
        {
            double sum = 0;
            string UserId = Session["UserId"].ToString();
            var cart = db.Carts.Where(c => c.UserId.Equals(UserId)).ToList();
            foreach (Cart c in cart)
            {
                sum = sum + c.Amount;
            }
            ViewBag.TotalSum = sum;
            var user = db.Users.Where(u => u.UserId.Equals(UserId)).FirstOrDefault();
            ViewBag.Address = user.Address + "\n" + user.City + "\n" + user.Country;
            ViewBag.Name = user.Firstname + " " + user.Lastname;
            return View(cart);
        }
        [Authorize]
        public ActionResult PayAll()
        {
            string UserId = Session["UserId"].ToString();
            var cart = db.Carts.Where(c => c.UserId.Equals(UserId)).ToList();
            var t = db.Transactions.Select(tn => tn.BillNo).DefaultIfEmpty(0).Max() + 1;
            Bill newbill = new Bill();
            newbill.BillNo = t;
            db.Bills.Add(newbill);
            db.SaveChanges();
            double sum = 0;

            for (int i = 0; i < cart.Count; i++)
            {
                Transaction Trx = new Transaction();
                Trx.BillNo = t;
                Trx.UserId = cart[i].UserId;
                var c = cart[i].ProductId.ToString();
                Trx.ProductId = c;
                Trx.ProductName = cart[i].ProductName;
                Trx.NoofProduct = cart[i].NoofProduct;
                Trx.Amount = cart[i].Amount;
                sum = sum + Trx.Amount;
                Trx.Tdate = DateTime.Now;
                db.Transactions.Add(Trx);
                db.SaveChanges();

                var p = db.Products.Where(pro => pro.Productid.Equals(c)).FirstOrDefault();
                p.Stock = p.Stock - cart[i].NoofProduct;
                p.SoldUnits = p.SoldUnits + 1;
                db.Entry(p).State = EntityState.Modified;
                db.SaveChanges();
                db.Carts.Remove(cart[i]);
                db.SaveChanges();
                ViewBag.Tid = Trx.Tid;

            }

            newbill.Amount = sum;
            db.Entry(newbill).State = EntityState.Modified;
            db.SaveChanges();
            ViewBag.TotalSum = newbill.Amount.ToString();
            return View();
        }
        public ActionResult PayNow(string UserId, string ProductId)
        {
            if (String.IsNullOrEmpty(ProductId))
            {
                ViewBag.Error = "Empty";
            }
            else
            {
                var c = db.Carts.Where(cart => cart.UserId.Equals(UserId) && cart.ProductId.Equals(ProductId)).FirstOrDefault();

                var p = db.Products.Where(product => product.Productid.Equals(ProductId)).FirstOrDefault();
                if (c.NoofProduct > p.Stock)
                {
                    ViewBag.Error = "No stock available";
                }
                else
                {
                    Transaction Trx = new Transaction();
                    Bill newbill = new Bill();
                    var t = db.Bills.Select(tn => tn.BillNo).DefaultIfEmpty(0).Max() + 1;
                    newbill.BillNo = t;
                    newbill.Amount = c.Amount;
                    db.Bills.Add(newbill);
                    db.SaveChanges();

                    Trx.BillNo = t;
                    Trx.UserId = c.UserId;
                    Trx.ProductId = c.ProductId;
                    Trx.ProductName = c.ProductName;
                    Trx.NoofProduct = c.NoofProduct;
                    Trx.Amount = c.Amount;
                    Trx.Tdate = DateTime.Now;
                    db.Transactions.Add(Trx);
                    db.SaveChanges();
                    p.Stock = p.Stock - c.NoofProduct;
                    p.SoldUnits = p.SoldUnits + 1;
                    db.Entry(p).State = EntityState.Modified;
                    db.SaveChanges();
                    db.Carts.Remove(c);
                    db.SaveChanges();
                    ViewBag.TotalSum = newbill.Amount.ToString();
                    ViewBag.Tid = Trx.Tid;
                }
            }
            return View();

        }
        public ActionResult CreditCard(int? Tid)
        {
            var transaction = db.Transactions.Where(t => t.Tid==Tid).FirstOrDefault();
            ViewBag.Deliverydate = transaction.Tdate.AddDays(5).ToShortDateString();
            return View();
        }
        public ActionResult DebitCard(int? Tid)
        {
            var transaction = db.Transactions.Where(t => t.Tid==Tid).FirstOrDefault();
            ViewBag.Deliverydate = transaction.Tdate.AddDays(5).ToShortDateString();
            return View();
        }
        public ActionResult NetBanking(int? Tid)
        {
            var transaction = db.Transactions.Where(t => t.Tid==Tid).FirstOrDefault();
            ViewBag.Deliverydate = transaction.Tdate.AddDays(5).ToShortDateString();
            return View();
        }
    }
}
