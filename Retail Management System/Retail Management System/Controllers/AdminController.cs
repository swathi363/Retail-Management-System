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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Retail_Management_System.Controllers
{
    public class AdminController : Controller
    {
        RetailContext db = new RetailContext();
        // GET: Admin
        [Authorize]
        public ActionResult Index()
        {
            return View(db.Products.ToList());
        }
        
             
        
        [Authorize]
        
        public ActionResult Details(string ProductId)
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
            return View(product);
        }
        [Authorize]
        public ActionResult CreateSupplier()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSupplier([Bind(Include = "SupplierId,SupplierName")]Supplier supplier)
        {
            if(ModelState.IsValid)
            {
                db.Suppliers.Add(supplier);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(supplier);
        }
        public ActionResult SupplierView()
        {
            return View(db.Suppliers.ToList());
        }
        [Authorize]
        public ActionResult EditSupplier(string SupplierId)
        {
            if (SupplierId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            Supplier supplier = db.Suppliers.Find(SupplierId);
            if (supplier == null)
            {
                return HttpNotFound();

            }
            return View(supplier);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSupplier([Bind(Include = "SupplierId,SupplierName")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                db.Entry(supplier).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            return View(supplier);

        }
        
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DetailsSuppliers(string SupplierId)
        {
            if(SupplierId==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            Supplier supplier = db.Suppliers.Find(SupplierId);
            if(supplier==null)
            {
                return HttpNotFound();

            }
            return View(supplier);

        }
        [Authorize]
        public ActionResult DeleteSupplier(string SupplierId)
        {
            if (SupplierId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            Supplier supplier = db.Suppliers.Find(SupplierId);
            if (supplier == null)
            {
                return HttpNotFound();

            }
            return View(supplier);
        }
        public ActionResult DeleteSupplierConfirmed(string SupplierId)
        {
            Supplier supplier = db.Suppliers.Find(SupplierId);
            db.Entry(supplier).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
                
        }
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.Supplier = db.Suppliers;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "Productid,ProductName,CategoryName,BrandName,PreferredAge,PreferredGender,Price,Stock,SoldUnits,Discount,SupplierId,Description")]Product product,HttpPostedFileBase file)
        {
            var path = "";
            if(file!=null)
            {
                if(file.ContentLength>0)
                {
                    if(Path.GetExtension(file.FileName).ToLower()==".jpg"
                        ||Path.GetExtension(file.FileName).ToLower()==".png"
                        || Path.GetExtension(file.FileName).ToLower() == ".gif"
                        || Path.GetExtension(file.FileName).ToLower() == ".jpeg")
                    {
                        path = Path.Combine(Server.MapPath("~/IMAGES/"), product.Productid+".jpg");
                        file.SaveAs(path);
                        ViewBag.UploadSuccess = true;
                    }

                }
            }
            if (ModelState.IsValid)
            {
                product.SpecialDiscount = 0;
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);

        }
        [Authorize]
        public ActionResult Edit(string ProductId)
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
            return View(product);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Productid,ProductName,CategoryName,BrandName,PreferredAge,PreferredGender,Price,Stock,SoldUnits,Discount,SpecialDiscount,SupplierId,Description")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }
        [Authorize]
        public ActionResult Delete(string ProductId)
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
            return View(product);
        }
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string ProductId)
        {
            Product product = db.Products.Find(ProductId);
            product.Stock = 0;
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Admin adm)
        {
            adm.Password = encrypt(adm.Password);
            var admin = db.Admins.Where(a => a.UserId.Equals(adm.UserId) && a.Password.Equals(adm.Password)).FirstOrDefault();
            if (admin != null)
            {
                FormsAuthentication.SetAuthCookie(admin.UserId, false);
                Session["UserId"] = admin.UserId.ToString();
                Session["Username"] = (admin.Firstname + " " + admin.Lastname).ToString();
                return RedirectToAction("Index");

            }
            else
            {
                ModelState.AddModelError("", "Invalid Login Credentials");
            }
            return View(adm);
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
        public ActionResult Feedback()
        {
                return View(db.Feedbacks.ToList());
        }
        [Authorize]
        public ActionResult Report()
        {
                return View();
            
        }
        [Authorize]
        [HttpPost]
        public ActionResult Report(string TDate)
        {
            DateTime dt;
            if (DateTime.TryParse(TDate, out dt))
            {
                var trs = db.Transactions.ToList();
                for (int i = 0; i < trs.Count; i++)
                {
                    if (trs[i].Tdate.Month != dt.Month)
                    {
                        trs.Remove(trs[i]);
                        i--;
                    }
                }
                return View("ReportView", trs);
            }
            else
            {
                return RedirectToAction("Report");
            }
        }
       
        public ActionResult AddAdmin()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAdmin([Bind(Include = "UserId,Firstname,Lastname,Password,ConfirmPassword,ContactNumber")]Admin adm)
        {
            adm.Password = encrypt(adm.Password);
            adm.ConfirmPassword = encrypt(adm.ConfirmPassword);
            var check = db.Users.Find(adm.UserId);
            if (check == null)
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                db.Admins.Add(adm);
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
        public ActionResult EditAdmin()
        {
            string username = User.Identity.Name;
            Admin user = db.Admins.FirstOrDefault(u => u.UserId.Equals(username));
            Admin model = new Admin();
            model.Firstname = user.Firstname;
            model.Lastname = user.Lastname;
            model.ContactNumber = user.ContactNumber;
            return View(model);

        }
        //Post Edit Current user info
        [Authorize]
        [HttpPost]
        public ActionResult EditAdmin(Admin usr)
        {
            string username = User.Identity.Name;
            Admin user = db.Admins.FirstOrDefault(u => u.UserId.Equals(username));
            user.Firstname = usr.Firstname;
            user.Lastname = usr.Lastname;
            user.ContactNumber = usr.ContactNumber;
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
        public ActionResult ChangePassword(Admin usr)
        {
            usr.Password = encrypt(usr.Password);
            usr.ConfirmPassword = encrypt(usr.ConfirmPassword);
            string username = User.Identity.Name;
            Admin user = db.Admins.FirstOrDefault(u => u.UserId.Equals(username));
            user.Password = usr.Password;
            user.ConfirmPassword = usr.ConfirmPassword;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        //Dispose the database
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}