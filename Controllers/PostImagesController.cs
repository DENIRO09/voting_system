using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using E_Lect.Models;
using System.IO;

namespace E_Lect.Controllers
{
    public class PostImagesController : Controller
    {
        private ELectDBEntities db = new ELectDBEntities();

        // GET: PostImages
        public ActionResult Index()
        {
            return View(db.SaveImages.ToList());
        }


        public ActionResult AdminViewIndex()
        {
            return View(db.PostImages.ToList());
        }


        public FileStreamResult RenderImage(int id)
        {
            MemoryStream ms = null;

            var item = db.SaveImages.Find(id);
            if (item != null)
            {
                ms = new MemoryStream(item.Image);
            }
            return new FileStreamResult(ms, item.ImagePath);
        }

        public FileStreamResult RenderImage2(int id)
        {
            MemoryStream ms = null;

            var item = db.PostImages.Find(id);
            if (item != null)
            {
                ms = new MemoryStream(item.Image);
            }
            return new FileStreamResult(ms, item.ImagePath);
        }

        // GET: PostImages/Details/5


        public byte[] ConvertToBytes(HttpPostedFileBase file)
        {
            BinaryReader reader = new BinaryReader(file.InputStream);
            return reader.ReadBytes((int)file.ContentLength);
        }


        // GET: PostImages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PostImages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PostImageID,Image,ImagePath")] PostImage postImage,HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                postImage.ImagePath = Path.GetExtension(file.FileName);
                postImage.Image = ConvertToBytes(file);
            }
            int number = 0;
            var num = db.PostImages.ToList();
            foreach (var n in num)
            {
                number++;
            }
            if (number == 0)
            {
                postImage.PostImageID = 0;
            }
            else
            {
                postImage.PostImageID=number;
            }
            if (ModelState.IsValid)
            {


                db.PostImages.Add(postImage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(postImage);
        }

        // GET: PostImages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PostImage postImage = db.PostImages.Find(id);
            if (postImage == null)
            {
                return HttpNotFound();
            }
            return View(postImage);
        }

        // POST: PostImages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PostImageID,Image,ImagePath")] PostImage postImage)
        {
            if (ModelState.IsValid)
            {
                SaveImage save = new SaveImage();
                int number = 0;
                var num = db.SaveImages.ToList();
                foreach (var n in num)
                {
                    number++;
                }
                if (number == 0)
                {
                    save.SaveImageID = 0;
                }
                else
                {
                    save.SaveImageID = number;
                }
                save.Image = postImage.Image;
                save.ImagePath = postImage.ImagePath;
                db.SaveImages.Add(save);
                db.SaveChanges();



                PostImage c = db.PostImages.Find(postImage.PostImageID);
                db.PostImages.Remove(c);
                db.SaveChanges();
                return RedirectToAction("Index","PostImages");

                //db.Entry(postImage).State = EntityState.Modified;
                //db.SaveChanges();
                //return RedirectToAction("Index");
            }
            return View(postImage);
        }

        public ActionResult ValidateImages(int id)
        {
            var postImage = db.PostImages.Find(id);
            SaveImage save = new SaveImage();
            int number = 0;
            var num = db.SaveImages.ToList();
            foreach (var n in num)
            {
                number++;
            }
            if (number == 0)
            {
                save.SaveImageID = 0;
            }
            else
            {
                save.SaveImageID = number;
            }
            save.Image = postImage.Image;
            save.ImagePath = postImage.ImagePath;
            db.SaveImages.Add(save);
            db.SaveChanges();



            PostImage c = db.PostImages.Find(postImage.PostImageID);
            db.PostImages.Remove(c);
            db.SaveChanges();
            return RedirectToAction("AdminViewIndex", "PostImages");
        }

        // GET: PostImages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PostImage postImage = db.PostImages.Find(id);
            if (postImage == null)
            {
                return HttpNotFound();
            }
            return View(postImage);
        }

        // POST: PostImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PostImage postImage = db.PostImages.Find(id);
            db.PostImages.Remove(postImage);
            db.SaveChanges();
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
    }
}
