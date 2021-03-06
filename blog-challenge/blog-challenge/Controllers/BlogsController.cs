﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using blog_challenge.Models;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace blog_challenge.Controllers
{
    public class BlogsController : Controller
    {
        private blogEntities1 db = new blogEntities1();
      
        // GET: Blogs
        public ActionResult Index()

        {

            return View(db.Blog.ToList());
        }

        // GET: Blogs/Details/5
        public ActionResult Details(int? id)
        {
            HttpPostedFileBase file = Request.Files["ImageData"];
            byte[] imageBytes = ConvertToBytes(file);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blog.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // GET: Blogs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Blogs/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Bid,Bttitle,Bcontent,BCategory,BDate,Bimagen")] Blog blog)
        {
            
            HttpPostedFileBase file = Request.Files["ImageData"];
            if(file!=null)
            { 
            byte[] imageBytes = ConvertToBytes(file);
                blog.Bimagen = imageBytes;
            }
          
            if (ModelState.IsValid)
            {
                blog.Bactive = true;
               
                db.Blog.Add(blog);
                db.SaveChanges();
                return RedirectToAction("Index");

            }

            return View(blog);
        }

        // GET: Blogs/Edit/5
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Blog blog = db.Blog.Find(id);


            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // POST: Blogs/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Bid,Bttitle,Bcontent,BCategory,BDate,Bimagen")] Blog blog)
        {
            if (Request.Files["ImageData"].ContentLength >0 )
            {
                HttpPostedFileBase file = Request.Files["ImageData"];
                byte[] imageBytes = ConvertToBytes(file);
                blog.Bimagen = imageBytes;

            }
           else
            {
                using (var db = new blogEntities1())
                {

                    var blogDb = db.Blog.Find(blog.Bid);
                    blog.Bimagen = blogDb.Bimagen;
                   
                }
               

            }
            if (ModelState.IsValid)
            {
             
                
                db.Entry(blog).State = EntityState.Modified;
                blog.Bactive = true;
               
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blog);
        }

        // GET: Blogs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blog.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {/*
          Blog blog = db.Blog.Find(id);
           db.Blog.Remove(blog);
            var search = from s in db.Blog select s;
              Blog bg = new Blog();
            bg.Bactive = false;
            */
            using (var db = new blogEntities1())
            {
                
                var user = db.Blog.Find(id);
                user.Bactive = false;
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            //db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        //GET: Blogs
        /*  public ActionResult search(int? id)
          {
              var busqueda = from s in db.Blog select s;

              if (id!=null)
              {
                  busqueda = busqueda.Where(s => s.Bid == id);
              }
              return View(busqueda.ToList());
          }
         */
        public ActionResult search(string tittle)
        {
            var search = from s in db.Blog select s;

            if (!String.IsNullOrEmpty(tittle))
            {
                search = search.Where(s => s.Bttitle.Contains(tittle));
            }
            return View(search.ToList());
        }
        public ActionResult Deleted(int? id)
        {
            return View(db.Blog.ToList());

        }
        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        public ActionResult Restore(int? id)
        {
            using (var db = new blogEntities1())
            {
                var user = db.Blog.Find(id);
                user.Bactive = true;
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            //db.SaveChanges();
            return RedirectToAction("Index");
        }


        private byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            if (image != null)
            {
                byte[] imageBytes;
                BinaryReader reader = new BinaryReader(image.InputStream);
                imageBytes = reader.ReadBytes((int)image.ContentLength);
                return imageBytes;
            }
            else
            {
                return null;
            }

        }
        [AllowAnonymous]
        public ActionResult RetrieveImage(int id)
        {
            Blog bgI = db.Blog.Find(id);

            byte[] cover = bgI.Bimagen;

            if (cover != null)
            {
                return File(cover, "image/jpg");
            }
            else
            {
                return null;
            }
        }

    }
}
