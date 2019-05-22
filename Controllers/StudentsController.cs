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
using System.Web.Helpers;

namespace E_Lect.Controllers
{
    public class StudentsController : Controller
    {
        private ELectDBEntities db = new ELectDBEntities();

        // GET: Students
        public ActionResult Index()
        {
            var time = db.VoteTimes.Find(1);

            if ((time.Start_Date <= DateTime.Now.Date) && DateTime.Now.Date < time.End_Date)
            {
                RedirectToAction("Index");
            }

            else if (DateTime.Now.Date > time.Start_Date)
            {
                return RedirectToAction("Construct");
            }

            else if (time.End_Date == DateTime.Now.Date)
            {
                return RedirectToAction("Construct");
            }

            else
            {
                return RedirectToAction("Construct");
            }

            return View(db.Students.ToList());
        }

        public ActionResult Construct()
        {
            return View();
        }

        public ActionResult GetData()
        {
            db.Configuration.ProxyCreationEnabled = false;

            List<Student> studentList = db.Students.ToList<Student>();
            return Json(new { data = studentList }, JsonRequestBehavior.AllowGet);
        }

        // GET: Students/Details/5
        public ActionResult Details(int? id)
        {
            id = (int)Session["Student_Number"];

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        public byte[] ConvertToBytes(HttpPostedFileBase file)
        {
            BinaryReader reader = new BinaryReader(file.InputStream);
            return reader.ReadBytes((int)file.ContentLength);
        }

        public FileStreamResult RenderImage(int id)
        {
            MemoryStream ms = null;

            var item = db.Students.Find(id);
            if (item != null)
            {
                ms = new MemoryStream(item.Student_Image);
            }
            return new FileStreamResult(ms, item.Student_ImagePath);
        }

        public ActionResult _Details(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        //public ActionResult ViewModel()
        //{
        //    Student data =new Student();
        //    List<ViewVote> vv = new List<ViewVote>();
        //    var List = (from c in db.Candidates
        //                join p in db.Parties on c.Party_Name equals p.Party_Name
        //                select new
        //                {
        //                    c.Candidate_ID,
        //                    c.Candidate_Name,
        //                    c.Candidate_MidName,
        //                    c.Candidate_Surname,
        //                    c.Candidate_Image,
        //                    c.Candidate_ImagePath,
        //                    c.Candidate_Position
        //                ,
                            
        //                    p.Party_Name,
        //                    p.Party_Image,
        //                    p.Party_ImagePath
        //                }).ToList();
        //    foreach (var item in List)
        //    {
        //        ViewVote lm = new ViewVote();
               
        //        lm.Candidate_Name = item.Candidate_Name;
        //        lm.Candidate_MidName = item.Candidate_MidName;
        //        lm.Candidate_Surname = item.Candidate_Surname;
        //        lm.Candidate_Position = item.Candidate_Position;
        //        lm.Candidate_Image = item.Candidate_Image;
        //        lm.Candidate_ImagePath = item.Candidate_ImagePath;
        //        lm.Party_ID = item.Party_ID;
        //        lm.Party_Name = item.Party_Name;
        //        lm.Party_Image = item.Party_Image;
        //        lm.Party_ImagePath = item.Party_ImagePath;
        //        vv.Add(lm);
        //    }
        //    return View(vv);
        //}

        // GET: Students/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Student_Number,Student_Password,ConfirmPassword,Student_Email,Student_Gender,Student_DOB,Student_Course,Student_YearOfStudy,Student_Campus,Student_Voted,Student_RecoveryQ,Student_RecoveryA,Student_Image,Student_ImagePath")] Student student,HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                student.Student_ImagePath = Path.GetExtension(file.FileName);
                student.Student_Image = ConvertToBytes(file);
            }
            if (ModelState.IsValid)
            {
                student.Student_Voted = "False";

                if (db.Students.Any(x => x.Student_Number == student.Student_Number))
                {
                    ModelState.Clear();
                    ViewBag.DuplicateMessage = "You are already registered.";
                    return View("Create", student);
                }

                var studentNum = db.MockStudents.Find(student.Student_Number);

                if (studentNum == null)
                {
                    ModelState.Clear();
                    ViewBag.ErrorMessage = "Invalid student! You are not a registered DUT student!";
                    return View("Create", student);
                }

                //student.Student_Recovery = " ";

                db.Students.Add(student);
                db.SaveChanges();
                try
                {
                    WebMail.SmtpServer = "smtp.gmail.com";
                    WebMail.SmtpPort = 587;
                    WebMail.SmtpUseDefaultCredentials = true;
                    WebMail.EnableSsl = true;
                    WebMail.UserName = "";
                    WebMail.Password = "";
                    WebMail.From = "denirophekoo@gmail.com";
                    WebMail.Send(to: student.Student_Email, subject: "Good day applicant"+student.Student_Number, body: "Your registration, to the greatest voting app to be developed, has been successfully completed. Thank you for choosing E-Lect. You have made the right choice! if you keep making choices like this you are going places in life. \n"+"Thanking you Profoundly \n"+"The E-Lect Team", isBodyHtml: true);
                }
                catch(Exception)
                {

                }
                ModelState.Clear();
                ViewBag.SuccessMessage = "Registration Successful.";
                return View("Create", new Student());

                //return RedirectToAction("Index");
            }

            return View(student);
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int? id)
        {
            id = (int)Session["Student_Number"];

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Student_Number,Student_Password,ConfirmPassword,Student_Email,Student_Gender,Student_DOB,Student_Course,Student_YearOfStudy,Student_Campus,Student_Voted,Student_RecoveryQ,Student_RecoveryA,Student_Image,Student_ImagePath")] Student student,HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                student.Student_ImagePath = Path.GetExtension(file.FileName);
                student.Student_Image = ConvertToBytes(file);
            }
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details");
            }
            return View(student);
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
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
