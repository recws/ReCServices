using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Headers;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using ReCServices.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Http.Cors;
using Microsoft.Owin.Security.Infrastructure;
using System.Web;


namespace ReCServices.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ServiciosApiController : ApiController
    {
        ///// <summary>
        ///// Application DB context
        ///// </summary>
        //protected ApplicationDbContext ApplicationDbContext = new ApplicationDbContext();

        ///// <summary>
        ///// User manager - attached to application DB context
        ///// </summary>
        //protected UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ApplicationDbContext));
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        WS_CONTEXT db = new WS_CONTEXT();

        [Authorize]
        [HttpPost]
        [Route("api/ServiciosApi/WS_GPS_InsertaSimple/")]
        public RespuestaServicio WS_GPS_InsertaSimple([FromBody]EventoSimple eventosimple)
        {
            Negocio negocio = new Negocio();
            return negocio.InsertaSimple(eventosimple, "RestApi");
        }


        [HttpGet]
        [Route("api/ServiciosApi/WS_GPS_ObtieneNombreDominio/")]
        public string WS_GPS_ObtieneNombreDominio()
        {
            return HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
        }

        [HttpGet]
        [Route("api/ServiciosApi/WS_GPS_ConsultaUltimaPosicion/")]
        public IEnumerable<WS_GPS_ConsultaUltimaPosicion_Result> WS_GPS_ConsultaUltimaPosicion(string IMEI)
        {
            return db.WS_GPS_ConsultaUltimaPosicion(IMEI).AsEnumerable();
        }
        [HttpGet]
        [Route("api/ServiciosApi/WS_GPS_ConsultaHistoricoUTC/")]
        public IEnumerable<WS_GPS_ConsultaHistoricoUTC_Result> WS_GPS_ConsultaHistoricoUTC(string IMEI, DateTime FechaInicioUTC, DateTime FechaFinUTC)
        {
            return db.WS_GPS_ConsultaHistoricoUTC(IMEI, FechaInicioUTC, FechaFinUTC).AsEnumerable();
        }

        [HttpGet]
        [Route("api/ServiciosApi/WS_GPS_TruncaReCLog/")]
        public string TruncaReCLog()
        {
            string Estatus = "";
            string sPath = "";
            sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/") + "ReC.log";

            long filelength = new System.IO.FileInfo(sPath).Length / 1024;
            //Copia el archivo con otro nombre

            try
            {
                try
                {
                    System.IO.File.Copy(sPath, System.Web.Hosting.HostingEnvironment.MapPath("~/") + "ReC_" + DateTime.Now.ToString("ddMMyyyyHHmm") + ".log");
                }
                catch (Exception Ex)
                {
                }

                
                try
                {
                    using (var fs = new System.IO.FileStream(sPath, System.IO.FileMode.Truncate))
                    {
                    }
                    Estatus += "Truncado 1 correcto ReC.Log" + "(" + filelength.ToString() + ")";
                }
                catch (Exception Ex) {
                    Estatus += "Truncado 1 error ReC.Log" + "(" + filelength.ToString() + ")";
                }

                try
                {
                    System.IO.File.WriteAllBytes(sPath, new byte[0]);
                    Estatus += "Truncado 2 correcto ReC.Log" + "(" + filelength.ToString() + ")";
                }
                catch (Exception Ex) {
                    Estatus += "Truncado 2 error ReC.Log" + "(" + filelength.ToString() + ")";
                }

                try
                {
                    System.IO.File.WriteAllText(sPath, string.Empty);
                    Estatus += "Truncado 3 correcto ReC.Log" + "(" + filelength.ToString() + ")";
                }
                catch (Exception Ex) {
                    Estatus += "Truncado 3 error ReC.Log" + "(" + filelength.ToString() + ")";
                }

                try
                {
                    System.IO.FileStream fs = new System.IO.FileStream(sPath, System.IO.FileMode.OpenOrCreate);

                        // Set the length to 250Kb
                        Byte[] bytes = new byte[fs.Length];
                        fs.Read(bytes, 0, (int)fs.Length);
                        fs.Close();
                        System.IO.FileStream fs2 = new System.IO.FileStream(sPath, System.IO.FileMode.Create);
                        fs2.Write(bytes, (int)bytes.Length - 0, 0);
                        fs2.Flush();
                    Estatus += "Truncado 4 correcto ReC.Log" + "(" + filelength.ToString() + ")";
                }
                catch (Exception Ex)
                {
                    Estatus += "Truncado 4 error ReC.Log" + "(" + filelength.ToString() + ")";
                }

                return Estatus;
            }
            catch (Exception Ex)
            {
                return "Error al truncar el archivo ReC.Log " + Ex.Message + Estatus;
            }
        }


        [Authorize]
        [HttpGet]
        [Route("api/ServiciosApi/WS_GPS_ConsultaxUsuarioUTC/")]
        public string WS_GPS_ConsultaxUsuarioUTC(string Usuario, DateTime FechaInicioUTC, DateTime FechaFinUTC)
        {
            string userlogged = RequestContext.Principal.Identity.Name;
            //ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == ID);
            //var user = ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
            //var user = UserManager.FindById(User.Identity.GetUserId());

            //ApplicationDbContext context = new ApplicationDbContext("WS_CONTEXT_PROD");
            //var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            //ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());
            //string Username = currentUser.UserName;
            var dias = (FechaInicioUTC - FechaFinUTC);
            if (dias.TotalDays > 3 || dias.TotalDays < -3)
            {
                return "Error: Solo se permite un rango maximo de 3 dias.";
            }

            if (userlogged == "WS_4GPS" && (Usuario=="WS_SEND" || Usuario == "WS_NewPick" || Usuario == "WS_SagCargo")) {

            }
            else
            {
                log.Error("Error: No tiene permisos para este recurso. Contacte con el administrador." + userlogged + ". " + Usuario + ". ");
                return "Error: No tiene permisos para este recurso. Contacte con el administrador.";
            }

            string result = JsonConvert.SerializeObject(db.WS_GPS_ConsultaxUsuarioUTC(Usuario, FechaInicioUTC, FechaFinUTC).AsEnumerable());
            return result;
        }


        public class Login
        {
            //public string grant_type { get; set; }
            public string Usuario { get; set; }
            public string Password { get; set; }
        }
        public class LoginModel
        {
            public string grant_type { get; set; }
            public string username { get; set; }
            public string password { get; set; }
        }

        //[HttpPost]
        //[Route("api/ServiciosApi/ObtenerToken/")]
        //public async Task<JObject> ObtenerToken([FromBody]Login login)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        //setup client
        //        client.BaseAddress = new Uri("http://wstest.recsolutions.tech");
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //        //setup login data
        //        var formContent = new FormUrlEncodedContent(new[]
        //        {
        //            new KeyValuePair<string, string>("grant_type", "password"),
        //            new KeyValuePair<string, string>("username", login.Usuario),
        //            new KeyValuePair<string, string>("password", login.Password),
        //        });

        //        //send request
        //        HttpResponseMessage responseMessage = await client.PostAsync("/Token", formContent);

        //        //get access token from response body
        //        var responseJson = await responseMessage.Content.ReadAsStringAsync();
        //        var jObject = JObject.Parse(responseJson);
        //        return jObject;
        //    }
        //}

        //[HttpPost]
        //[Route("api/ServiciosApi/CerrarSesion/")]
        //public string CerrarSesion()
        //{
        //    //Esto no funciona, hay que recibir como paremtro el clientid o username
        //    Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
        //    return "Desconectado";
        //}

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

    }
}