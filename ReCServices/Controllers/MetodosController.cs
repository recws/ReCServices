using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReCServices.Controllers
{
    public class MetodosController : Controller
    {
        // GET: Metodos
        public ActionResult Index()
        {
            return View();
        }
        public string ObtenerNombreDominio()
        {
            return HttpRuntime.AppDomainAppVirtualPath.ToString();
        }
    }
}