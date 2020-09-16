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
                Session["Username"] = (user.Firstname + " " + user.Lastname).ToString();
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
                return RedirectToAction("Login");
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
            return RedirectToAction("Login");
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
        //Post Edit Current user info
        [Authorize]
        [HttpPost]
        public ActionResult Edit(User usr)
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
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }
        //Post change password for user
        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(User usr)
        {
            usr.Password = encrypt(usr.Password);
            usr.ConfirmPassword = encrypt(usr.ConfirmPassword);
            string username = User.Identity.Name;
            User user = db.Users.FirstOrDefault(u => u.UserId.Equals(username));
            user.Password = usr.Password;
            user.ConfirmPassword = usr.ConfirmPassword;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Settings");
        }
        [Authorize]
        [HttpGet]
        public ActionResult Cart()
        {
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
    }
}