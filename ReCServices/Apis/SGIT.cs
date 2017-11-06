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
using System.IO;
using System.Net;

namespace ReCServices.Apis
{
    public class SGIT
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void SGIT_ObtenerPosicion(string UsuarioReC, string Usuario, string Password)
        {

            var responseJson = "";
            try
            {
                string sPath = "";
                sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/" + "SOAPENV\\SOAPENV_ISLAS.txt");

                //XmlDocument doc = new XmlDocument();
                //doc.Load(@"\test_ws.txt");
                //string xmlcontents = doc.InnerXml;
                string xmlcontents = File.ReadAllText(sPath);
                
                xmlcontents = xmlcontents.Replace("[-pwd-]", Password);
                xmlcontents = xmlcontents.Replace("[-user-]", Usuario);



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

                        var rawXML = XDocument.Parse(soapResult);

                        var returnResult = (from r in rawXML.Descendants("item") select r).ToList();

                        foreach (XElement xElement in returnResult)
                        {
                            //var Imei = xElement.Element("idgps") != null ? xElement.Element("idgps").Value : "";
                            //if (Imei == null)
                            //{
                            //    continue;
                            //}
                            //var Respuesta = xElement.Element("Respuesta") != null ? xElement.Element("Respuesta").Value : "";
                            //var UnitPlate = xElement.Element("UnitPlate") != null ? xElement.Element("UnitPlate").Value : "";
                            //var Latitude = xElement.Element("Latitude") != null ? xElement.Element("Latitude").Value : "";
                            //var Longitude = xElement.Element("Longitude") != null ? xElement.Element("Longitude").Value : "";
                            //var Odometer = xElement.Element("Odometer") != null ? xElement.Element("Odometer").Value : "";
                            //var SpeedGps = xElement.Element("SpeedGps") != null ? xElement.Element("SpeedGps").Value : "";
                            //var Course = xElement.Element("Course") != null ? xElement.Element("Course").Value : "";
                            //var Ignition = xElement.Element("Ignition") != null ? xElement.Element("Ignition").Value : "";
                            //var DateGps = xElement.Element("DateGps") != null ? xElement.Element("DateGps").Value : "";
                            //var PanicButton = xElement.Element("PanicButton") != null ? xElement.Element("PanicButton").Value : "";
                            var Latitud = xElement.Element("Latitud") != null ? xElement.Element("Latitud").Value : "";
                            

                            if (Latitud == "")
                            {

                            }


                        }
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
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(@" http://avl.homelinux.net/satelital/wsdl/service.php?wsdl");
            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }


    }
}