using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E_Lect.Models;

namespace E_Lect.Controllers
{
    public class TotalVotesController : Controller
    {
        ELectDBEntities db = new ELectDBEntities();

        // GET: TotalVotes
        public ActionResult Index()
        {
            return View();
        }
        [ChildActionOnly]
        public ActionResult ListPartyPV()
        {
            return PartialView("_PartyPV", db.Parties.ToList());
        }

        [ChildActionOnly]
        public ActionResult ListCandidatePV()
        {
            return PartialView("_CandidatePV", db.Candidates.ToList());
        }
    }
}