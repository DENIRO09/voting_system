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
using Newtonsoft.Json;
using System.IO;

namespace E_Lect.Controllers
{
    public class AdminsController : Controller
    {
        private ELectDBEntities db = new ELectDBEntities();

        // GET: Admins
        public ActionResult Index()
        {
            return View(db.Admins.ToList());
        }

        public ActionResult GetData()
        {
            db.Configuration.ProxyCreationEnabled = false;

            List<Admin> adminList = db.Admins.ToList<Admin>();
            return Json(new { data = adminList }, JsonRequestBehavior.AllowGet);
        }

        public byte[] ConvertToBytes(HttpPostedFileBase file)
        {
            BinaryReader reader = new BinaryReader(file.InputStream);
            return reader.ReadBytes((int)file.ContentLength);
        }
        public FileStreamResult RenderImage(String id)
        {
            MemoryStream ms = null;

            var item = db.Admins.Find(id);
            if (item != null)
            {
                ms = new MemoryStream(item.Admin_Image);
            }
            return new FileStreamResult(ms, item.Admin_ImagePath);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new Admin());
        }

        [HttpPost]
        public ActionResult Create(Admin admin,HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                admin.Admin_ImagePath = Path.GetExtension(file.FileName);
                admin.Admin_Image = ConvertToBytes(file);
            }
            if (db.Admins.Any(x => x.Admin_Email == admin.Admin_Email) || db.Admins.Any(x => x.Admin_Username == admin.Admin_Username))
            {
                return Json(new { error = true, message = "Admin Username already exists!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                db.Admins.Add(admin);
                db.SaveChanges();
                return Json(new { success = true, message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
            }
            
        }

        // GET: Admins/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Admin admin = db.Admins.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // GET: Admins/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Admin admin = db.Admins.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // POST: Admins/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Admin_Username,Admin_Password,Confirm_Password,Admin_Name,Admin_MidName,Admin_Surname,Admin_DOB,Admin_Gender,Admin_Email,Admin_RecoveryQ,Admin_RecoveryA,Admin_Image,Admin_ImagePath")] Admin admin,HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                admin.Admin_ImagePath = Path.GetExtension(file.FileName);
                admin.Admin_Image = ConvertToBytes(file);
            }
                db.Entry(admin).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
                Admin admin = db.Admins.Where(x => x.Admin_Username == id).FirstOrDefault<Admin>();

                db.Admins.Remove(admin);
                db.SaveChanges();
                return Json(new { success = true, message = "Party Deleted Successfully" }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult BackupAdmin()
        {
            StreamWriter file = new StreamWriter(Server.MapPath("~/adminbackup.txt"), true);
            var list = db.Admins.ToList();
            foreach (var item in list)
            {
                file.WriteLine(item.Admin_Username + "#" + item.Admin_Name + "#" + item.Admin_MidName + "#" + item.Admin_Surname + "#" + item.Admin_Email + "#" + item.Admin_Gender + "#" + item.Admin_DOB + "#" + item.Admin_Password + "#" + item.ConfirmPassword + "#" + item.Admin_RecoveryQ + "#" + item.Admin_RecoveryA);


            }
            file.Close();
            return View("Home");
        }

        public ActionResult BackupStudents()
        {
            StreamWriter file = new StreamWriter(Server.MapPath("~/studentbackup.txt"), true);
            var list = db.Students.ToList();
            foreach (var item in list)
            {
                file.WriteLine(item.Student_Number + "#" + item.Student_Password + "#" + item.ConfirmPassword + "#" + item.Student_Email + "#" + item.Student_Gender + "#" + item.Student_DOB + "#" + item.Student_Course + "#" + item.Student_Voted + "#" + item.Student_RecoveryQ + "#" + item.Student_RecoveryA );


            }
            file.Close();
            return View("Home");
        }

        public ActionResult BackupCandidates()
        {
            StreamWriter file = new StreamWriter(Server.MapPath("~/candidatebackup.txt"), true);
            var list = db.Candidates.ToList();
            foreach (var item in list)
            {
                file.WriteLine(item.Candidate_ID + "#" + item.Candidate_Name + "#" + item.Candidate_MidName + "#" + item.Candidate_Surname + "#" + item.Candidate_StudentNumber + "#" + item.Candidate_Gender + "#" + item.Candidate_DOB + "#" + item.Candidate_Email + "#" + item.Candidate_Course + "#" + item.Candidate_YearOfStudy + "#" + item.Candidate_Campus + "#" + item.Candidate_Position + "#" + item.Candidate_Status + "#" + item.Candidate_TotalVotes + "#" + item.Party_Name + "#" + item.Admin_Username + "#" + item.Candidate_Image + "#" + item.Candidate_ImagePath);


            }
            file.Close();
            return View("Home");
        }

        public ActionResult BackupParty()
        {
            StreamWriter file = new StreamWriter(Server.MapPath("~/partybackup.txt"), true);
            var list = db.Parties.ToList();
            foreach (var item in list)
            {
                file.WriteLine(item.Party_Name  + "#" + item.Party_Description + "#" + item.Party_NoOfCandidates + "#" + item.Party_Status + "#" + item.Party_TotalVotes + "#" + item.Admin_Username );


            }
            file.Close();
            return View("Home");
        }

        public ActionResult BackupVotes()
        {
            StreamWriter file = new StreamWriter(Server.MapPath("~/votesbackup.txt"), true);
            var list = db.Votes.ToList();
            foreach (var item in list)
            {
                file.WriteLine(item.Vote_ID + "#" + item.Party_Name + "#" + item.Candidate_ID + "#" + item.Student_Number + "#" + item.Vote_Date);


            }
            file.Close();
            return View("Home");
        }

    }
}
