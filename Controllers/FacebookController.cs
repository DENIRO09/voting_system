using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace E_Lect.Controllers
{
    public class FacebookController : Controller
    {
        // GET: Facebook
        public ActionResult Index()
        {
            string fbAuthCode = Request["code"]; //The authorization code.
            string fbAppId = "denirophekoo@gmail.com"; //Your fb application id.
            string fbSecretAppId = "Deniro09"; //Your fb secret app id, it is found on the fb application configuration page.
            string redirectUrl = string.Format("[THE_REDIRECT_PAGE]", "https://www.facebook.com/E-lect-969591966513164/"); //The redirect url. THIS MUST BE THE EXACT SAME REDIRECT URL USED ON THE JAVASCRIPT LINK!
            string fbUrl = "https://graph.facebook.com/oauth/access_token?client_id=" + fbAppId + "&redirect_uri=" + redirectUrl + "&client_secret=" + fbSecretAppId + "&code=" + fbAuthCode; //Url used to post.
            string accessToken = string.Empty;

            try
            {
                WebClient client = new WebClient();
                using (Stream stream = client.OpenRead(fbUrl))
                using (StreamReader reader = new StreamReader(stream))
                {
                    accessToken = reader.ReadToEnd().Split('&')[0].Replace("access_token=", string.Empty);
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error ocurred while trying to get the fb token in " + fbUrl, ex);
            }






            string postUrl = "https://graph.facebook.com/me/feed";
            string postParameters;

            postParameters = string.Format("message={0}&picture={1}&name={2}&caption={2}&description={3}&link={4}&access_token={5}",
                                                "[Message]",
                                                "[PictureUrl]",
                                                "[Name]",
                                                "[Caption]",
                                                "[Link]",
                                                accessToken);

            try
            {
                System.Net.WebRequest req = System.Net.WebRequest.Create(postUrl);

                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";

                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(postParameters);
                req.ContentLength = bytes.Length;
                using (System.IO.Stream os = req.GetRequestStream())
                {
                    os.Write(bytes, 0, bytes.Length); //Push it out there
                    os.Close();
                    using (WebResponse resp = req.GetResponse())
                    using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                    {
                        ViewBag.PostResult = sr.ReadToEnd().Trim();
                        sr.Close();
                    }
                    os.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error ocurred while posting data to the user's wall: " + postUrl + "?" + postParameters, ex);
            }


            return View();
        }
    }
}