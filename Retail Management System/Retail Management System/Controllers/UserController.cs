using Microsoft.Ajax.Utilities;
using Retail_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Retail_Management_System.Controllers
{
    public class UserController : Controller
    {
        RetailContext db = new RetailContext();

        public ActionResult Index(string category)
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(User usr)
        {
            usr.Password = encrypt(usr.Password);
            var user = db.Users.Where(a => a.UserId.Equals(usr.UserId) && a.Password.Equals(usr.Password)).FirstOrDefault();
            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(usr.UserId, false);
                Session["UserId"] = user.UserId.ToString();
                Session["Username"] = (user.Firstname).ToString();
                Session["Role"] = "user";
                return RedirectToAction("Index","Product");

            }
            else
            {
                ModelState.AddModelError("", "Invalid Login Credentials");
            }
            return View(usr);
        }

       
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "UserId,Firstname,Lastname,Password,ConfirmPassword,Address,ContactNumber,City,Country")] User usr)
        {
            usr.Password = encrypt(usr.Password);
            usr.ConfirmPassword = encrypt(usr.ConfirmPassword);
            var check = db.Users.Find(usr.UserId);
            if (check == null)
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                db.Users.Add(usr);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "User already Exists");
                return View();
            }
        }
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index");
        }
        public string encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (System.IO.MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        [Authorize]
        public ActionResult Edit()
        {
            if (Session["UserId"] != null && Session["Role"].ToString() == "user")
            {
            
                string username = User.Identity.Name;
                User user = db.Users.FirstOrDefault(u => u.UserId.Equals(username));
                User model = new User();
                model.Firstname = user.Firstname;
                model.Lastname = user.Lastname;
                model.Address = user.Address;
                model.ContactNumber = user.ContactNumber;
                model.City = user.City;
                model.Country = user.Country;
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "User");

            }

        }
        //Post Edit Current user info
        [Authorize]
        [HttpPost]
        public ActionResult Edit(User usr)
        {
            if (Session["UserId"] != null && Session["Role"].ToString() == "user")
            {
            
                string username = User.Identity.Name;
                User user = db.Users.FirstOrDefault(u => u.UserId.Equals(username));
                user.Firstname = usr.Firstname;
                user.Lastname = usr.Lastname;
                user.Address = usr.Address;
                user.ContactNumber = usr.ContactNumber;
                user.City = usr.City;
                user.Country = usr.Country;
                user.Password = user.Password;
                user.ConfirmPassword = user.Password;
                Session["Username"] = (user.Firstname + " " + user.Lastname).ToString();
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return View(usr);
            }
            else
            {
                return RedirectToAction("Index", "User");
            }
        }
        [Authorize]
        public ActionResult ChangePassword()
        {
            if (Session["UserId"] != null && Session["Role"].ToString() == "user")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "User");
            }
        }
        //Post change password for user
        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(User usr)
        {
            if (Session["UserId"] != null && Session["Role"].ToString() == "user")
            {
            
            
            
                usr.Password = encrypt(usr.Password);
                usr.ConfirmPassword = encrypt(usr.ConfirmPassword);
                string username = User.Identity.Name;
                User user = db.Users.FirstOrDefault(u => u.UserId.Equals(username));
                user.Password = usr.Password;
                user.ConfirmPassword = usr.ConfirmPassword;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index", "User");

            }
        }
        [Authorize]
        [HttpGet]
        public ActionResult Cart()
        {
            if(Session["UserId"]==null)
            {
                return RedirectToAction("Index");
            }
            string id = (string)Session["UserId"];

            var carts = db.Carts.Where(c => c.UserId.Equals(id)).ToList();
            if (carts != null)
            {
                return View(carts);
            }
            else
            {
                ViewBag.Error = "Cart Empty";
            }
            return View();
        }
        [Authorize]
        public ActionResult CartDelete(string UserId, string ProductId)
        {
            if (Session["UserId"] != null && Session["Role"].ToString() == "user")
            {
            
            
            
                if (ProductId == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Cart cart = db.Carts.Where(c => c.UserId.Equals(UserId) && c.ProductId.Equals(ProductId)).FirstOrDefault();
                if (cart == null)
                {
                    return HttpNotFound();
                }
                return View(cart);
            }
            else
            {
                return RedirectToAction("Index", "User");
            }
        }
        //Post delete cart details
        [Authorize]
        [HttpPost, ActionName("CartDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string UserId, string ProductId)
        {
            if (Session["UserId"] != null && Session["Role"].ToString() == "user")
            {
               
           
            
                Cart cart = db.Carts.Where(c => c.UserId.Equals(UserId) && c.ProductId.Equals(ProductId)).FirstOrDefault();
                db.Carts.Remove(cart);
                db.SaveChanges();
                return RedirectToAction("Cart");
            }
            else 
            {
                return RedirectToAction("Index", "User");
            }
        }
        [Authorize]
        public ActionResult MyOrders()
        {
            if (Session["UserId"] != null && Session["Role"].ToString() == "user")
            {

                string UserId = Session["UserId"].ToString();
                var orders = db.Transactions.Where(c => c.UserId.Equals(UserId)).ToList().OrderBy(o => o.Tdate).ToList();
                return View(orders);
            }
            else
            {
                return RedirectToAction("Index");
            }
            
        }
        [Authorize]
        public ActionResult CancelOrder(int? TId)
        {
            if (Session["UserId"] != null && Session["Role"].ToString() == "user")
            {



                if (TId == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var transaction = db.Transactions.Where(t => t.Tid == TId).FirstOrDefault();
                if (transaction == null)
                {
                    return HttpNotFound();
                }
                return View(transaction);
            }
            else
            {
                return RedirectToAction("Index", "User");
            }

        }
        [Authorize]
        [HttpPost, ActionName("CancelOrder")]
        [ValidateAntiForgeryToken]
        public ActionResult CancelallConfirmed(int TId, int BillNo)
        {
            if (Session["UserId"] != null && Session["Role"].ToString() == "user")
            {
            
            
            
                var transaction = db.Transactions.Find(TId);
                var ProductId = transaction.ProductId;
                var products = db.Products.Find(ProductId);
                products.Stock = products.Stock + transaction.NoofProduct;
                products.SoldUnits = products.SoldUnits + 1;
                db.Entry(products).State = EntityState.Modified;
                db.SaveChanges();
                double Amount = transaction.Amount;
                db.Transactions.Remove(transaction);
                db.SaveChanges();
                var bill = db.Bills.Find(BillNo);
                bill.Amount = bill.Amount - Amount;
                db.Entry(bill).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MyOrders");
            }
            else
            {
                return RedirectToAction("Index", "User");
            }
        }
        [Authorize]
        public ActionResult Feedback(string ProductId,string productname)
        {
            if (Session["UserId"] != null && Session["Role"].ToString() == "user")
            {
            
                ViewBag.ProductId = ProductId;
                ViewBag.ProductName = productname;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "User");

            }
        }
        [Authorize]
        [HttpPost]
        public ActionResult Feedback([Bind(Include = "Feedback")] Feedback fed, string ProductId)
        {
            if (Session["UserId"] != null && Session["Role"].ToString() == "user")
            {
            
                fed.ProductId = ProductId;
                fed.UserId = Session["UserId"].ToString();
                db.Feedbacks.Add(fed);
                db.SaveChanges();
                return RedirectToAction("MyOrders");
            }
            else
            {
                return RedirectToAction("Index", "User");

            }
        }
    }
}