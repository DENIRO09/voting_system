using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E_Lect.Models;
using System.Web.Helpers;

namespace E_Lect.Controllers
{
    public class ChartController : Controller
    {
        ELectDBEntities db = new ELectDBEntities();
        // GET: Chart

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BarCampus()
        {
            decimal ritson = 0;
            decimal biko = 0;
            decimal ml = 0;
            decimal city = 0;
            decimal pmb = 0;

            var Lists = db.Students.ToList();
            foreach (var item in Lists)
            {
                string campus = item.Student_Campus;
                if (campus == "Ritson")
                {
                    ritson++;
                }
                else if (campus == "ML Sultan")
                {
                    ml++;

                }
                else if (campus == "Biko")
                {
                    biko++;
                }
                else if (campus == "City")
                {
                    city++;
                }
                else if (campus == "PMB")
                {
                    pmb++;
                }

            }

            var c = new Chart(width: 800, height: 200)
                      .AddSeries(
                      chartType: "column",
                      xValue: new[] { "Ritson", "Ml", "Biko", "City", "PMB" },
                      yValues: new[] { ritson, ml, biko, city, pmb })
                      .Write("png");

            return null;

        }

        public ActionResult BarPartyVotes()
        {
            decimal ancyl = 0;
            decimal effsc = 0;
            decimal sasco = 0;
            decimal daso = 0;

            var Lists = db.Votes.ToList();

            foreach (var item in Lists)
            {
                string campus = item.Party_Name;
                if (campus == "ANCYL")
                {
                    ancyl++;
                }
                else if (campus == "EFFSC")
                {
                    effsc++;

                }
                else if (campus == "SASCO")
                {
                    sasco++;
                }
                else if (campus == "DASO")
                {
                    daso++;
                }

            }

            var c = new Chart(width: 800, height: 200)
                      .AddSeries(
                      chartType: "column",
                      xValue: new[] { "ANCYL", "EFFSC", "SASCO", "DASO" },
                      yValues: new[] { ancyl, effsc, sasco, daso })
                      .Write("png");

            return null;

        }



        public ActionResult BarVotes()
        {

            string[] partynames=new string[db.Parties.Count()];
            Decimal[] votes = new Decimal[db.Parties.Count()];
            int count = 0;
            var list = db.Parties.ToList();
            foreach(var item in list)
            {
                partynames[count] = item.Party_Name;
                votes[count]= Convert.ToDecimal(item.Party_TotalVotes);
                count++;
            }
           
         

            var c = new Chart(width: 800, height: 200)
                      .AddSeries(
                      chartType: "column",
                      xValue: new[] { partynames},
                      yValues: new[] {votes})
                      .Write("png");

            return null;

        }
    }
}