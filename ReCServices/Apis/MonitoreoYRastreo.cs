using System;
using System.Text;
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
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
//using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using System.Xml.Serialization;

namespace ReCServices.Apis
{
    public class MonitoreoYRastereo
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        public static void MonitoreoYRastreo_ObtenerPosicion(string UsuarioReC, string Usuario, string Password, string proveedorGPS)
        {
            var ECOenCurso = "";
            DataTable DT_Data = new DataTable();

            try
            {
                //Carga los IMEI que se van a consultar
                DT_Data = GetData_ListaGPSxProveedor(proveedorGPS); //tmp para no modificar demaciado el codigo (x8)
                if (DT_Data.Rows.Count == 0)
                {
                    return;
                }

                for (int i = 0; i < DT_Data.Rows.Count; i++)
                {
                    ECOenCurso = DT_Data.Rows[i]["Economico"].ToString();

                    var responseJson = "";
                    try
                    {
                        string sPath = "";
                        sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/" + "SOAPENV\\SOAPENV_DIEZ.txt");

                        //XmlDocument doc = new XmlDocument();
                        //doc.Load(@"\test_ws.txt");
                        //string xmlcontents = doc.InnerXml;
                        string xmlcontents = File.ReadAllText(sPath);
                        xmlcontents = xmlcontents.Replace("[-placa-]", ECOenCurso);
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
                                    var Imei = xElement.Element("idgps") != null ? xElement.Element("idgps").Value : "";
                                    if (Imei == null || Imei =="") {
                                        continue;
                                    }
                                    var Respuesta = xElement.Element("Respuesta") != null ? xElement.Element("Respuesta").Value : "";
                                    var UnitPlate = xElement.Element("UnitPlate") != null ? xElement.Element("UnitPlate").Value : "";
                                    var Latitude = xElement.Element("Latitude") != null ? xElement.Element("Latitude").Value : "";
                                    var Longitude = xElement.Element("Longitude") != null ? xElement.Element("Longitude").Value : "";
                                    var Odometer = xElement.Element("Odometer") != null ? xElement.Element("Odometer").Value : "";
                                    var SpeedGps = xElement.Element("SpeedGps") != null ? xElement.Element("SpeedGps").Value : "";
                                    var Course = xElement.Element("Course") != null ? xElement.Element("Course").Value : "";
                                    var Ignition = xElement.Element("Ignition") != null ? xElement.Element("Ignition").Value : "";
                                    var DateGps = xElement.Element("DateGps") != null ? xElement.Element("DateGps").Value : "";                                    
                                    var PanicButton = xElement.Element("PanicButton") != null ? xElement.Element("PanicButton").Value : "";



                                    try
                                    {
                                        //Consulta si ya existe la posicion, por si es repetida y no ha actualizado el equipo

                                        //string imei = Imei; //este dato ya viene arriba

                                        string lat = Latitude;
                                        string lng = Longitude;
                                        string codigoevento = "";//PanicButton == "true" ? "panic" : "";
                                        string odometro = Odometer.Split('.')[0];
                                        ////string placas = ((dynamic)res[i]).Plates;
                                        string velocidad = SpeedGps.Split('.')[0];
                                        string bateria = "100";
                                        string direccion = Course.Split('.')[0]; ;

                                        var fechahoragps = DateTime.ParseExact(DateGps, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture); ;
                                        fechahoragps = fechahoragps.ToUniversalTime();

                                        ////Validaciones

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

                                        WS_GPS_InsertaSimple = db.WS_GPS_InsertaSimple(UsuarioReC, Imei, codigoevento, LAT, LNG, "", true, VELOCIDAD, DIRECCION, BATERIA, ODOMETRO, fechahoragps, fechahoragps).ToList();
                                        if (WS_GPS_InsertaSimple[0].Indicador == 1)
                                        {

                                        }
                                        else
                                        {
                                            log.Error("Error al Insertar evento de " + UsuarioReC + ". IMEI: " + (Imei != null ? Imei : "") + ". PLACA: " + (UnitPlate != null ? UnitPlate : "")  + "  -  " + WS_GPS_InsertaSimple[0].Mensaje + ". " + responseJson);
                                        }
                                    }
                                    catch (Exception Ex)
                                    {
                                        log.Error("Error MonitoreoYRastreo_ObtenerPosicion: " + UsuarioReC + ". " + xElement.ToString() + ". " + Ex.Message);
                                    }
                                }

                                //FORMA 1 DE DESERIALIZAR
                                //XmlDocument document = new XmlDocument();
                                //document.LoadXml(soapResult);
                                //System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(SOAPClass.Envelope));
                                //SOAPClass.Envelope envelope = (SOAPClass.Envelope)serializer.Deserialize(new StringReader(document.OuterXml));


                                //FORMA 2 DE DESERIALIZAR
                                //SOAPClass.Envelope deserializedObject;
                                //using (var reader = rawXML.CreateReader(System.Xml.Linq.ReaderOptions.None))
                                //{
                                //    var ser = new XmlSerializer(typeof(SOAPClass.Envelope));
                                //    deserializedObject = (SOAPClass.Envelope)ser.Deserialize(reader);
                                //    var z = deserializedObject;
                                //}                                
                            }
                        }


                    }
                    catch (Exception Ex)
                    {
                        if (Ex.Message == "'System.Dynamic.ExpandoObject' no contiene una definición para 'Latitude'.")
                        {
                            //No guarda nada en el log por que aveces no viene completa la trama
                        }
                        if (Ex.Message == "Error en el servidor remoto: (403) Prohibido.")
                        {
                            //No guarda nada en el log por que aveces no viene completa la trama
                        }
                        else
                        {
                            log.Error("Error MonitoreoYRastreo_ObtenerPosicion: " + UsuarioReC + ". " + responseJson + ". " + Ex.Message);
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
                    log.Error("Error GrupoGCP_ObtenerPosicion: " + UsuarioReC + ". " + Ex.Message);
                }
            }
        }

        public static T SOAPToObject<T>(string SOAP)
        {
            if (string.IsNullOrEmpty(SOAP))
            {
                throw new ArgumentException("SOAP can not be null/empty");
            }
            using (MemoryStream Stream = new MemoryStream(UTF8Encoding.UTF8.GetBytes(SOAP)))
            {
                SoapFormatter Formatter = new SoapFormatter();
                return (T)Formatter.Deserialize(Stream);
            }
        }


        public static DataTable GetData_ListaGPSxProveedor(string Proveedor)
        {
            DataTable dtbl = new DataTable("DataTable1");

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WS_PROD"].ConnectionString);
            SqlDataAdapter adp = new SqlDataAdapter();
            adp.SelectCommand = new SqlCommand();
            adp.SelectCommand.Connection = con;

            adp.SelectCommand.CommandText = "WS_GPS_ListaGPSxProveedor";
            adp.SelectCommand.CommandType = CommandType.StoredProcedure;
            adp.SelectCommand.Parameters.AddWithValue("@IdTransportista", 0);
            adp.SelectCommand.Parameters.AddWithValue("@Proveedor", Proveedor);
            adp.Fill(dtbl);

            return dtbl;
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