using accountAuthentication.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace accountAuthentication.Controllers
{
    public class PhotoController : Controller
    {
        // GET: Photo
        NewDb2Entities db = new NewDb2Entities();
        // GET: Home
        public ActionResult Index(string searchBy, string search)
        {
            if (searchBy == "Image_Name")
                return View(db.Tables.Where(model => model.Image_Name == search).ToList());
            else if (searchBy == "Category")
                return View(db.Tables.Where(model => model.Category == search).ToList());
            else
            {
                var data = db.Tables.ToList();

                return View(data);
            }
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Table table)
        {
            string fileName = Path.GetFileNameWithoutExtension(table.ImageFile.FileName);
            string extension = Path.GetExtension(table.ImageFile.FileName);
            fileName = fileName + extension;
            table.image_Path = "~/images/" + fileName;
            fileName = Path.Combine(Server.MapPath("~/images/"), fileName);
            table.ImageFile.SaveAs(fileName);

            db.Tables.Add(table);

            int a = db.SaveChanges();
            if (a > 0)
            {
                ViewBag.Message = "<script>alert('Record Inserted')</script>";
                ModelState.Clear();
            }
            else
            {
                ViewBag.Message = "<script>alert('Record Not Inserted')</script>";
            }
            return RedirectToAction("Index","Photo");
        }
        public ActionResult Details(int Id)
        {
            var photoDetail = db.Tables.Where(model => model.Id == Id).FirstOrDefault();
            Session["Image2"] = photoDetail.image_Path;
            return View(photoDetail);

        }

        public ActionResult Edit(int id)
        {
            var EmployeeRow = db.Tables.Where(model => model.Id == id).FirstOrDefault();
            Session["Image"] = EmployeeRow.image_Path;
            return View(EmployeeRow);
        }

        [HttpPost]
        public ActionResult Edit(Table data)
        {
            if (ModelState.IsValid == true)
            {
                if (data.ImageFile != null)
                {

                    string fileName = Path.GetFileNameWithoutExtension(data.ImageFile.FileName);
                    string extension = Path.GetExtension(data.ImageFile.FileName);
                    HttpPostedFileBase postedFile = data.ImageFile;
                    int length = postedFile.ContentLength;

                    if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                    {
                        if (length <= 1000000)
                        {
                            fileName = fileName + extension;
                            data.image_Path = "~/images/" + fileName;
                            fileName = Path.Combine(Server.MapPath("~/images/"), fileName);
                            data.ImageFile.SaveAs(fileName);
                            db.Entry(data).State = EntityState.Modified;
                            int a = db.SaveChanges();
                            if (a > 0)
                            {
                                string ImagePath = Request.MapPath(Session["Image"].ToString());
                                if (System.IO.File.Exists(ImagePath))
                                {
                                    System.IO.File.Delete(ImagePath);
                                }
                                TempData["UpdateMessage"] = "<script>alert('Data Updated Successfully.')</script>";
                                ModelState.Clear();
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                TempData["UpdateMessage"] = "<script>alert('Data Not Updated.')</script>";
                            }
                        }
                        else
                        {
                            TempData["SizeMessage"] = "<script>alert('Image Size should be less than 1 MB')</script>";
                        }
                    }
                    else
                    {
                        TempData["ExtensionMessage"] = "<script>alert('Format Not Supported')</script>";
                    }


                }
                else
                {
                    data.image_Path = Session["Image"].ToString();
                    db.Entry(data).State = EntityState.Modified;
                    int a = db.SaveChanges();
                    if (a > 0)
                    {
                        TempData["UpdateMessage"] = "<script>alert('Data Updated Successfully.')</script>";
                        ModelState.Clear();
                        return RedirectToAction("Index", "Photo");
                    }
                    else
                    {
                        TempData["UpdateMessage"] = "<script>alert('Data Not Updated.')</script>";
                    }

                }
            }
            return View();
        }

        public ActionResult Delete(int id)
        {
            if (id >= 0)
            {
                var data = db.Tables.Where(model => model.Id == id).FirstOrDefault();

                if (data != null)
                {
                    db.Entry(data).State = EntityState.Deleted;
                    int a = db.SaveChanges();
                    if (a > 0)
                    {
                        TempData["DeleteMessage"] = "<script>alert('Data Deleted Successfully.')</script>";
                        string ImagePath = Request.MapPath(data.image_Path.ToString());
                        if (System.IO.File.Exists(ImagePath))
                        {
                            System.IO.File.Delete(ImagePath);
                        }
                    }
                    else
                    {
                        TempData["DeleteMessage"] = "<script>alert('Data Not Deleted.')</script>";
                    }
                }
            }

            return RedirectToAction("Index", "Photo");
        }
    }
}