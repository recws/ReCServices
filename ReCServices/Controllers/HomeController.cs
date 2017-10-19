using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReCServices.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var currentURL = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Authority;

            ViewBag.Title = "Home Page";
            ViewBag.Domain = currentURL.ToString();

            return View();
        }
    }
}
