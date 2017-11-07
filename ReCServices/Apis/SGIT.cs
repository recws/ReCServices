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
using System.Xml.Serialization;
using System.Collections.Generic;

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

                        ////FORMA 1 DE DESERIALIZAR
                        //XmlDocument document = new XmlDocument();
                        //document.LoadXml(soapResult);
                        //System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(Envelope));
                        //Envelope envelope = (Envelope)serializer.Deserialize(new StringReader(document.InnerXml));


                        var rawXML = XDocument.Parse(soapResult);

                        var returnResult = (from r in rawXML.Descendants("item") select r).ToList();

                        foreach (XElement xElement in returnResult)
                        {
                            var returnResult2 = (from r2 in xElement.Descendants("item") select r2).ToList();
                            if (returnResult2.Count == 0)
                            {
                                continue;
                            }
                            string id= "0", lat="0", lng="0", codigoevento ="", odometro="0", velocidad="0", bateria="0", fechahoragps="",direccion = "0",placas="",Gps="";

                            foreach (XElement xElement2 in returnResult2)
                            {
                                var key = xElement2.Element("key") != null ? xElement2.Element("key").Value : "";
                                var value = xElement2.Element("value") != null ? xElement2.Element("value").Value : "";

                                if (key == "Id") {
                                    id = value.ToString();
                                }
                                else if (key == "Latitud")
                                {
                                    lat = value.ToString();
                                }
                                else if (key == "Longitud")
                                {
                                    lng = value.ToString();
                                }
                                else if (key == "Odometro")
                                {
                                    odometro = value.ToString().Split('.')[0]; ;
                                }
                                else if (key == "Velocidad")
                                {
                                    velocidad = value.ToString().Split('.')[0]; ;
                                }
                                //else if (key == "Curso")
                                //{
                                //    direccion = value.ToString().Split('.')[0]; ;
                                //}
                                else if (key == "Fecha")
                                {
                                    fechahoragps = value.ToString();
                                }
                                else if (key == "Placas")
                                {
                                    placas = System.Text.RegularExpressions.Regex.Replace(value.ToString(), "[^a-zA-Z0-9]", "");
                                }
                                else if (key == "Gps")
                                {
                                    Gps = value.ToString();
                                }
                            }


                            try
                            {
                                DateTime FechaHoraGPS = DateTime.Today;
                                if (fechahoragps.Length > 0)
                                {
                                    FechaHoraGPS = DateTime.ParseExact(fechahoragps, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                                    FechaHoraGPS = FechaHoraGPS.ToUniversalTime();
                                }
                                else {
                                    log.Error("Error SGIT_ObtenerPosicion: " + UsuarioReC + ". " + placas + ". " + "La fechahoragps es incorrecta");
                                    continue;
                                }

                                //////Conversiones de datos
                                var LAT = decimal.Parse(lat);
                                var LNG = decimal.Parse(lng);
                                var ODOMETRO = int.Parse(odometro);
                                ////var PLACAS = System.Text.RegularExpressions.Regex.Replace(placas, "-", "");
                                ////PLACAS = System.Text.RegularExpressions.Regex.Replace(PLACAS, " ", "");
                                var VELOCIDAD = int.Parse(velocidad);
                                var DIRECCION = int.Parse(direccion);
                                var BATERIA = int.Parse(bateria);


                                //Si no es repetida la inserta
                                List<WS_GPS_InsertaSimple_Result> WS_GPS_InsertaSimple;

                                WS_CONTEXT db = new WS_CONTEXT();

                                WS_GPS_InsertaSimple = db.WS_GPS_InsertaSimple(UsuarioReC, placas, codigoevento, LAT, LNG, "", true, VELOCIDAD, DIRECCION, BATERIA, ODOMETRO, FechaHoraGPS, FechaHoraGPS).ToList();
                                if (WS_GPS_InsertaSimple[0].Indicador == 1)
                                {

                                }
                                else
                                {
                                    log.Error("Error al Insertar evento de " + UsuarioReC + ". IMEI: " + (placas != null ? placas : "").ToString() + "  -  " + WS_GPS_InsertaSimple[0].Mensaje + ". " + responseJson);
                                }
                            }
                            catch (Exception Ex)
                            {
                                log.Error("Error SGIT_ObtenerPosicion: " + UsuarioReC + ". " + xElement.ToString() + ". " + Ex.Message);
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



        [XmlRoot(ElementName = "key")]
        public class Key
        {
            [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
            public string Type { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "value")]
        public class Value
        {
            [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
            public string Type { get; set; }
            [XmlText]
            public string Text { get; set; }
            [XmlElement(ElementName = "item")]
            public List<Item> Item { get; set; }
        }

        [XmlRoot(ElementName = "item")]
        public class Item
        {
            [XmlElement(ElementName = "key")]
            public Key Key { get; set; }
            [XmlElement(ElementName = "value")]
            public Value Value { get; set; }
        }

        [XmlRoot(ElementName = "arreglo")]
        public class Arreglo
        {
            [XmlElement(ElementName = "item")]
            public List<Item> Item { get; set; }
            [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
            public string Type { get; set; }
        }

        [XmlRoot(ElementName = "posicionResponse", Namespace = "urn:Localizacion")]
        public class PosicionResponse
        {
            [XmlElement(ElementName = "arreglo")]
            public Arreglo Arreglo { get; set; }
        }

        [XmlRoot(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public class Body
        {
            [XmlElement(ElementName = "posicionResponse", Namespace = "urn:Localizacion")]
            public PosicionResponse PosicionResponse { get; set; }
        }

        [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public class Envelope
        {
            [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
            public Body Body { get; set; }
            [XmlAttribute(AttributeName = "SOAP-ENV", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string SOAPENV { get; set; }
            [XmlAttribute(AttributeName = "ns1", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string Ns1 { get; set; }
            [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string Xsi { get; set; }
            [XmlAttribute(AttributeName = "xsd", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string Xsd { get; set; }
            [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string Ns2 { get; set; }
            [XmlAttribute(AttributeName = "SOAP-ENC", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string SOAPENC { get; set; }
            [XmlAttribute(AttributeName = "encodingStyle", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
            public string EncodingStyle { get; set; }
        }

    }
}