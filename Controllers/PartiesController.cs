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
    public class PartiesController : Controller
    {
        private ELectDBEntities db = new ELectDBEntities();

        // GET: Parties
        public ActionResult Index()
        {
            var parties = db.Parties.Include(p => p.Admin);
            return View(parties.ToList());
        }

        public ActionResult GetData()
        {
            db.Configuration.ProxyCreationEnabled = false;

            List<Party> partyList = db.Parties.ToList<Party>();
            return Json(new { data = partyList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _Index()
        {
            var parties = db.Parties.Include(p => p.Admin);
            return View(parties.ToList());
        }

        // GET: Parties/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Party party = db.Parties.Find(id);
            if (party == null)
            {
                return HttpNotFound();
            }
            return View(party);
        }

        // GET: Parties/Create
        public ActionResult Create()
        {
            ViewBag.Admin_Username = new SelectList(db.Admins, "Admin_Username", "Admin_Password");
            return View();
        }

        // POST: Parties/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Party party,HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                party.Party_ImagePath = Path.GetExtension(file.FileName);
                party.Party_Image = ConvertToBytes(file);
            }
            var admin = (string)Session["Admin_Username"];
                party.Admin_Username = admin;

                party.Party_NoOfCandidates = 0;
                party.Party_TotalVotes = 0;

            if (db.Parties.Any(x => x.Party_Name == party.Party_Name) || db.Parties.Any(x => x.Party_Description == party.Party_Description))
                {
                    return Json(new { error = true, message = "Party already exists!" }, JsonRequestBehavior.AllowGet);
                }

                db.Parties.Add(party);
                db.SaveChanges();
                return Json(new { success = true, message = "Party create Successfully" }, JsonRequestBehavior.AllowGet);
            
        }

        // GET: Parties/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Party party = db.Parties.Find(id);
            var studentNum = (string)Session["Admin_Username"];
            if(party.Admin_Username==studentNum)
            {
                return View(db.Parties.Where(x => x.Party_Name == id).FirstOrDefault<Party>());
            }
            else
            {
                //return ViewBag ayesha
            }
            if (party == null)
            {
                return HttpNotFound();
            }
            return View(party);
        }

        // POST: Parties/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Party party,HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                party.Party_ImagePath = Path.GetExtension(file.FileName);
                party.Party_Image = ConvertToBytes(file);
            }
            var admin = (string)Session["Admin_Username"];
            party.Admin_Username = admin;

            var partyInfo = db.Parties.Find(party.Party_Name);

            if (partyInfo != null)
            {
                party.Party_Name = partyInfo.Party_Name;
            }

            db.Entry(party).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
        }


        public byte[] ConvertToBytes(HttpPostedFileBase file)
        {
            BinaryReader reader = new BinaryReader(file.InputStream);
            return reader.ReadBytes((int)file.ContentLength);
        }
        public FileStreamResult RenderImage(String id)
        {
            MemoryStream ms = null;

            var item = db.Parties.Find(id);
            if (item != null)
            {
                ms = new MemoryStream(item.Party_Image);
            }
            return new FileStreamResult(ms, item.Party_ImagePath);
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            var admin = (string)Session["Admin_Username"];
          
            Party party = db.Parties.Where(x => x.Party_Name == id).FirstOrDefault<Party>();
            if (party.Admin_Username == admin)
            {
                db.Parties.Remove(party);
                db.SaveChanges();
                return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //ayesha viewbag
                //ayesha return it
            }
            return View(party);
        }

        // GET: Parties/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Party party = db.Parties.Find(id);
        //    if (party == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(party);
        //}

        //// POST: Parties/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Party party = db.Parties.Find(id);
        //    db.Parties.Remove(party);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
