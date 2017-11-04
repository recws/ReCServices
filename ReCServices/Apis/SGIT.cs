using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Xml;

namespace ReCServices.Apis
{
    public class SGIT
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        public static ReCServices.WS_SGIT_Service.LocalizacionPortClient ws = new ReCServices.WS_SGIT_Service.LocalizacionPortClient();
        
        public static string Token { get; set; }

        public static void SGIT_ObtenerPosicion(string UsuarioReC, string Usuario, string Password)
        {

             

            //try
            //{

            //    string respuesta = ws.posicion("wsantonio", "wsantonio01");
            //}
            //catch(Exception ex)
            //{
                
            //}

            
       
        }
    }
}