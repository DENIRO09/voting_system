using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using E_Lect.Models;
using System.Web.Helpers;

namespace E_Lect.Controllers
{
    public class VotesController : Controller
    {
        private ELectDBEntities db = new ELectDBEntities();

        public ActionResult EditVoteTime(int? id)
        {
            id = 1;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VoteTime voteTime = db.VoteTimes.Find(id);

            if (voteTime == null)
            {
                return HttpNotFound();
            }
            return View(voteTime);
        }

        // POST: VoteTimes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditVoteTime([Bind(Include = "ID,Start_Date,End_Date")] VoteTime voteTime)
        {
            if (ModelState.IsValid)
            {
                db.Entry(voteTime).State = EntityState.Modified;
                db.SaveChanges();

                ModelState.Clear();
                ViewBag.SuccessMessage = "Voting Time changed successfully!";
                return View("EditVoteTime");
            }
            return View(voteTime);
        }

        public ActionResult VoteSuccess()
        {
            return View();
        }

        public ActionResult Construct()
        {
            return View();
        }
        // GET: Votes
        public ActionResult Index()
        {
            var votes = db.Votes.Include(v => v.Candidate).Include(v => v.Party).Include(v => v.Student);
            return View(votes.ToList());
        }

        // GET: Votes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vote vote = db.Votes.Find(id);
            if (vote == null)
            {
                return HttpNotFound();
            }
            return View(vote);
        }
        
        // GET: Votes/Create
        public ActionResult Create()
        {
            var time = db.VoteTimes.Find(1);
            ProductModel product = new ProductModel();

            if ((time.Start_Date <= DateTime.Now.Date) && DateTime.Now.Date < time.End_Date)
            {
                using (ELectDBEntities db = new ELectDBEntities())
                {
                    List<Candidate> ac = new List<Candidate>();
                    var list = db.Candidates.ToList();
                    foreach (var item in list)
                    {
                        var index = db.Parties.Find(item.Party_Name);
                        if (index != null)
                        {

                            if (index.Party_Status == "Active" && item.Candidate_Status == "Active")
                            {
                                Candidate c = new Candidate();
                                c.Candidate_ID = item.Candidate_ID;
                                c.Candidate_Name = item.Candidate_Name;
                                c.Candidate_MidName = item.Candidate_MidName;
                                c.Candidate_Surname = item.Candidate_Surname;
                                c.Candidate_Position = item.Candidate_Position;
                                c.Candidate_Image = item.Candidate_Image;
                                c.Candidate_ImagePath = item.Candidate_ImagePath;
                                c.isChecked = item.isChecked;
                                ac.Add(c);
                            }
                        }
                    }

                    product.Cand = ac.ToList<Candidate>();
                }

                RedirectToAction("Create");

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

            //ViewBag.Candidate_ID = new SelectList(db.Candidates, "Candidate_ID", "Candidate_Name");
            //ViewBag.Party_ID = new SelectList(db.Parties, "Party_ID", "Party_Name");
            //ViewBag.Student_Number = new SelectList(db.Students, "Student_Number", "Student_Password");
            return View(product);
        }

        // POST: Votes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        public ActionResult Create(ProductModel product)
        {
            var studentNum = (int)Session["Student_Number"];

            if (db.Votes.Any(x => x.Student_Number == studentNum))
            {
                var selectedCandidate = product.Cand.Where(x => x.isChecked == true).ToList<Candidate>();

            if (selectedCandidate != null)
            {

                foreach (var item in selectedCandidate)
                {
                        int news = 0;
                        int number = 0;
                    var num = db.Votes.ToList();
                        if (num == null)
                        {
                            news = 1;
                        }
                        else
                        {
                            foreach (var n in num)
                            {
                                number++;
                            }


                            int count = 0;
                            foreach (var l in num)
                            {
                                count++;
                                if (count == number)
                                {
                                    news = l.Vote_ID + 1;
                                }
                            }

                        }
                        
                    var vote = new Vote();
                    vote.Vote_ID = news;
                    vote.Student_Number = studentNum;
                    vote.Candidate_ID = item.Candidate_ID;

                    ELectDBEntities sis = new ELectDBEntities();
                    Candidate cands = sis.Candidates.Find(vote.Candidate_ID);

                    if (cands != null)
                    {
                        cands.Candidate_TotalVotes += 1;
                        sis.Entry(cands).State = EntityState.Modified;
                        sis.SaveChanges();
                    }

                    else if (cands == null)
                    {
                        ModelState.Clear();
                        ViewBag.ErrorMessage = "Incorrect Candidate!";
                        return View("Create", vote);
                    }

                    var partyIDs = cands.Party_Name;
                    vote.Party_Name = partyIDs;

                    ELectDBEntities ses = new ELectDBEntities();
                    var partys = ses.Parties.Find(vote.Party_Name);

                    if (partys != null)
                    {
                        partys.Party_TotalVotes += 1;
                        ses.Entry(partys).State = EntityState.Modified;
                        ses.SaveChanges();
                    }

                    db.Votes.Add(vote);
                    db.SaveChanges();

                }
            }
            else
            {
                ViewBag.ErrorMessage="You have not selected any candidates to vote for";
                return View("Create");

            }
                ViewBag.DuplicateMessage = "You have already voted!";
                return View("Create", product);
            }
            
            try
            {
                var s = db.Students.Find(studentNum);
                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.SmtpPort = 587;
                WebMail.SmtpUseDefaultCredentials = true;
                WebMail.EnableSsl = true;
                WebMail.UserName = "";
                WebMail.Password = "";
                WebMail.From = "";
                WebMail.Send(to: s.Student_Email, subject: "Dearest applicant : " + s.Student_Number, body: "Your vote through the E-Lect online voting application has been successful. Please go ahead and share our apllication. Help a brother out. We appreciate your use of E-Lect. Voting is made easy through E-Lect.Viva E-Lect Viva! \n" + "Yours Gratefully \n" + "The E-Lect Team", isBodyHtml: true);
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Your internet connection slow and the email was not able to be sent to you.";

            }
           
            return View("VoteSuccess");
        }

        // GET: Votes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vote vote = db.Votes.Find(id);
            if (vote == null)
            {
                return HttpNotFound();
            }
            ViewBag.Candidate_ID = new SelectList(db.Candidates, "Candidate_ID", "Candidate_Name", vote.Candidate_ID);
            ViewBag.Party_ID = new SelectList(db.Parties, "Party_ID", "Party_Name", vote.Party_Name);
            ViewBag.Student_Number = new SelectList(db.Students, "Student_Number", "Student_Password", vote.Student_Number);
            return View(vote);
        }

        // POST: Votes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Vote_ID,Party_ID,Candidate_ID,Student_Number")] Vote vote)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vote).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Candidate_ID = new SelectList(db.Candidates, "Candidate_ID", "Candidate_Name", vote.Candidate_ID);
            ViewBag.Party_ID = new SelectList(db.Parties, "Party_ID", "Party_Name", vote.Party_Name);
            ViewBag.Student_Number = new SelectList(db.Students, "Student_Number", "Student_Password", vote.Student_Number);
            return View(vote);
        }

        // GET: Votes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vote vote = db.Votes.Find(id);
            if (vote == null)
            {
                return HttpNotFound();
            }
            return View(vote);
        }

        // POST: Votes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Vote vote = db.Votes.Find(id);
            db.Votes.Remove(vote);
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
