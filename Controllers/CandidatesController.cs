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
    public class CandidatesController : Controller
    {
        private ELectDBEntities db = new ELectDBEntities();

        // GET: Candidates
        public ActionResult Index()
        {
            var candidates = db.Candidates.Include(c => c.Admin).Include(c => c.Party);
            return View(candidates.ToList());
        }

        public ActionResult _Index()
        {
            var candidates = db.Candidates.Include(c => c.Admin).Include(c => c.Party);
            return View(candidates.ToList());
        }

        //DONT REMOVE
        public ActionResult GetData()
        {
            db.Configuration.ProxyCreationEnabled = false;

            List<Candidate> candList = db.Candidates.ToList<Candidate>();
            return Json(new { data = candList }, JsonRequestBehavior.AllowGet);
        }

        // GET: Candidates/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Candidate candidate = db.Candidates.Find(id);
            if (candidate == null)
            {
                return HttpNotFound();
            }
            return View(candidate);
        }

        public FileStreamResult RenderImage(String id)
        {
            MemoryStream ms = null;

            var item = db.Candidates.Find(id);
            if (item != null)
            {
                ms = new MemoryStream(item.Candidate_Image);
            }
            return new FileStreamResult(ms, item.Candidate_ImagePath);
        }

        public byte[] ConvertToBytes(HttpPostedFileBase file)
        {
            BinaryReader reader = new BinaryReader(file.InputStream);
            return reader.ReadBytes((int)file.ContentLength);
        }

        // GET: Candidates/Create
        public ActionResult Create()
        {
            ViewBag.Admin_Username = new SelectList(db.Admins, "Admin_ID", "Admin_Username");
            ViewBag.Party_ID = new SelectList(db.Parties, "Party_ID", "Party_Name");
            return View();
        }
        
        // POST: Candidates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Candidate candidate,HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                candidate.Candidate_ImagePath = Path.GetExtension(file.FileName);
                candidate.Candidate_Image = ConvertToBytes(file);
            }

            //if (ModelState.IsValid)
            //{
            candidate.Candidate_ID = candidate.GenerateCandID();
                candidate.Candidate_TotalVotes = 0;

                var admin = (string)Session["Admin_Username"];
                candidate.Admin_Username = admin;

                if (db.Candidates.Any(x => x.Candidate_ID == candidate.Candidate_ID) || db.Candidates.Any(x => x.Candidate_StudentNumber == candidate.Candidate_StudentNumber))
                {
                ModelState.Clear();
                ViewBag.DuplicateMessage = "This candidate already exists.";
                return View("Create");
            }


                var studentNum = db.MockStudents.Find(candidate.Candidate_StudentNumber);

                if (studentNum == null)
                {
                ModelState.Clear();
                ViewBag.ErrorMessage = "Invalid candidate! Not a registered DUT student!";
                return View("Create", candidate);
            }


                var party = db.Parties.Find(candidate.Party_Name);

                if (party != null)
                {
                    party.Party_NoOfCandidates += 1;
                    db.Entry(party).State = EntityState.Modified;
                    db.SaveChanges();
                }

                else if (party == null)
                {
                ModelState.Clear();
                ViewBag.ErrorMessage = "Incorrect Party!";
                return View("Create", candidate);
               
                }


                db.Candidates.Add(candidate);
                db.SaveChanges();

            ModelState.Clear();
            ViewBag.SuccessMessage = "Registration Successfully.";
            return View("Create", new Candidate());
        }

        // GET: Candidates/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Candidate candidate = db.Candidates.Find(id);

            var studentNum = (string)Session["Admin_Username"];

            if (candidate.Admin_Username == studentNum)
            {
                if (candidate == null)
                {
                    return HttpNotFound();
                }

                return View(db.Candidates.Where(x => x.Candidate_ID == id).FirstOrDefault<Candidate>());
            }
            else
            {
                //ayesha viewbag
            }
            return View(db.Candidates.Where(x => x.Candidate_ID == id).FirstOrDefault<Candidate>());
        }

        // POST: Candidates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Candidate candidate,HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                candidate.Candidate_ImagePath = Path.GetExtension(file.FileName);
                candidate.Candidate_Image = ConvertToBytes(file);
            }

                var admin = (string)Session["Admin_Username"];
            if (candidate.Admin_Username == admin)
            {   
                candidate.Admin_Username = admin;

                db.Entry(candidate).State = EntityState.Modified;
                db.SaveChanges();
              
            }
            else
            {
                return RedirectToAction("Index");
            }
            return View(candidate);
               
            
        }
        public ActionResult Delete(string id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Candidate candidate = db.Candidates.Find(id);
            var Admin = (string)Session["Admin_Username"];
            if (Admin == candidate.Admin_Username)
            {
                if (candidate == null)
                {
                    return HttpNotFound();
                }

                var party = db.Parties.Find(candidate.Party_Name);

                if (party != null)
                {
                    party.Party_NoOfCandidates -= 1;
                    db.Entry(party).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            else
            {
                //ayesha viewbag 
                return RedirectToAction("Index", "Candidate"); 
            }

            return View(candidate);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Candidate candidate = db.Candidates.Find(id);
            db.Candidates.Remove(candidate);
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
