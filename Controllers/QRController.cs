using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MessagingToolkit.QRCode.Codec.Data;
using MessagingToolkit.QRCode.Codec;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.UI.WebControls;
using E_Lect.Models;


namespace E_Lect.Controllers
{
    public class QRController : Controller
    {
        // GET: QR
        ELectDBEntities db = new ELectDBEntities();
        public ActionResult Index()
        {
            String av = "";
            using (ELectDBEntities db = new ELectDBEntities())
            {
                av = av + "Candidates Taking Part in this Election \n" + "\n";
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

                            av = av + ("Candidate ID : " + item.Candidate_ID + "       Candidate Name: " + item.Candidate_Name + "                        Party Name:" + item.Party_Name + "\n" + "\n");



                        }
                    }
                }





                QRCodeEncoder endcoder = new QRCodeEncoder();
                Bitmap img = endcoder.Encode(av);
                img.Save(Server.MapPath("~/img.jpg"), ImageFormat.Jpeg);
                return View();



            }



        }





        public ActionResult FacebookLink()
        {
                String av = "http://www.facebook.com/E-Lect-96951966513164/";
         





                QRCodeEncoder endcoder = new QRCodeEncoder();
                Bitmap img = endcoder.Encode(av);
                img.Save(Server.MapPath("~/facebook.jpg"), ImageFormat.Jpeg);
                return View();



            }



        }
    }

