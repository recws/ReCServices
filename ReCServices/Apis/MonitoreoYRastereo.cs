using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Net.Http;

using System.Xml.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Xml;
using System.IO;

namespace ReCServices.Apis
{
    public class MonitoreoYRastereo
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static async void MonitoreoYRastreo_ObtenerPosicion(string UsuarioReC, string Usuario, string Password)
        {
            var responseJson = "";
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(@"C:\documents\projects\test_ws.txt");
                string xmlcontents = doc.InnerXml;


                HttpWebRequest request = CreateWebRequest();
                XmlDocument soapEnvelopeXml = new XmlDocument();
                soapEnvelopeXml.LoadXml(xmlcontents);

                using (Stream stream = request.GetRequestStream())
                {
                    soapEnvelopeXml.Save(stream);
                }

                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        string soapResult = rd.ReadToEnd();
                        //Console.WriteLine(soapResult);
                    }
                }


            }
            catch (Exception Ex)
            {
                if (Ex.Message == "'System.Dynamic.ExpandoObject' no contiene una definición para 'Latitude'.")
                {
                    //No guarda nada en el log por que aveces no viene completa la trama
                }
                else
                {
                    log.Error("Error GrupoUDA_ObtenerPosicion: " + UsuarioReC + ". " + responseJson + ". " + Ex.Message);
                }
            }
        }

        public static HttpWebRequest CreateWebRequest()
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(@" http://45.55.209.120/sistema/soap/query.php?wsdl");
            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }
    }
}