using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E_Lect.Models;
using System.Web.Helpers;

namespace E_Lect.Controllers
{
    public class LoginController : Controller
    {

        ELectDBEntities db = new ELectDBEntities();
        // GET: AdminLogin
        public ActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AdminLogin(Admin admin)
        {
            using (ELectDBEntities db = new ELectDBEntities())
            {
                var username = db.Admins.Where(x => x.Admin_Username == admin.Admin_Username).FirstOrDefault();
                var password = db.Admins.Where(x => x.Admin_Password == admin.Admin_Password).FirstOrDefault();

                if (username == null && password == null)
                {
                    ViewBag.ErrorMessage = "Incorrect Username and Password";
                    return View("AdminLogin", admin);
                }

                else if (username == null)
                {
                    ViewBag.ErrorMessage = "Incorrect Username";
                    return View("AdminLogin", admin);
                }

                else if (password == null)
                {
                    ViewBag.ErrorMessage = "Incorrect Password";
                    return View("AdminLogin", admin);
                }

                else
                {
                    Session["Admin_Username"] = admin.Admin_Username;
                    return RedirectToAction("AdminHome", "Home");
                }
            }
        }

        public ActionResult AdminLogOff()
        {
            string username = (string)Session["Admin_Username"];
            Session.Abandon();
            return RedirectToAction("AdminLogin", "Login");
        }

        // GET: StudentLogin
        public ActionResult StudentLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult StudentLogin(Student student)
        {
            using (ELectDBEntities db = new ELectDBEntities())
            {
                var studentNum = db.Students.Where(x => x.Student_Number == student.Student_Number).FirstOrDefault();
                var password = db.Students.Where(x => x.Student_Password == student.Student_Password).FirstOrDefault();

                if (studentNum == null && password == null)
                {
                    ViewBag.ErrorMessage = "Incorrect Username and Password";
                    return View("StudentLogin", student);
                }

                else if (studentNum == null)
                {
                    ViewBag.ErrorMessage = "Incorrect Username";
                    return View("StudentLogin", student);
                }

                else if (password == null)
                {
                    ViewBag.ErrorMessage = "Incorrect Password";
                    return View("StudentLogin", student);
                }

                else
                {
                    Session["Student_Number"] = student.Student_Number;
                    return RedirectToAction("StudentHome", "Home");
                }
            }
        }

        public ActionResult StudentLogOff()
        {
            int studentNum = (int)Session["Student_Number"];
            Session.Abandon();
            return RedirectToAction("StudentLogin", "Login");
        }


        public ActionResult StudentForgotPassword(Student student)
        {
            var results = db.Students.Find(student.Student_Number);
            if (results != null)
            {
                try
                {
                    WebMail.SmtpServer = "smtp.gmail.com";
                    WebMail.SmtpPort = 587;
                    WebMail.SmtpUseDefaultCredentials = true;
                    WebMail.EnableSsl = true;
                    WebMail.UserName = "denirophekoo@gmail.com";
                    WebMail.Password = "09091998";
                    WebMail.From = "denirophekoo@gmail.com";
                    WebMail.Send(to: student.Student_Email, subject: "Good day applicant" + student.Student_Number, body: "Your password to login into our system is :  " + results.Student_Password, isBodyHtml: true);
                }
                catch (Exception)
                {

                }

                return RedirectToAction("StudentLogin", student);
            }
            else
            {
                //ayesha viewbag
            }
            return View();
        }


        public ActionResult AdminForgotPassword(Admin admin)
        {
            var results = db.Students.Find(admin.Admin_Username);
            if (results != null)
            {


                try
                {
                    WebMail.SmtpServer = "smtp.gmail.com";
                    WebMail.SmtpPort = 587;
                    WebMail.SmtpUseDefaultCredentials = true;
                    WebMail.EnableSsl = true;
                    WebMail.UserName = "denirophekoo@gmail.com";
                    WebMail.Password = "09091998";
                    WebMail.From = "denirophekoo@gmail.com";
                    WebMail.Send(to: admin.Admin_Email, subject: "Good day applicant" + admin.Admin_Username, body: "Your password to login into our system is :  " + results.Student_Password, isBodyHtml: true);
                }
                catch (Exception)
                {

                }

                return RedirectToAction("AdminLogin", admin);
            }
            else
            {
                //viewbag ayesha
            }
            return View();
        }
    }
}