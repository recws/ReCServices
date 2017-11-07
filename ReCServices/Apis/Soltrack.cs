using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;

namespace ReCServices.Apis
{
    public static class Soltrack
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static async void Soltrack_ObtenerPosicion(string UsuarioReC, string Usuario, string Password)
        {
            var responseJson = "";
            var IMEIenCurso = "";
            DataTable DT_Data = new DataTable();

            try
            {
                //Carga los IMEI que se van a consultar
                DT_Data = GetData_ListaGPSxProveedor("T. Hernandez");
                if (DT_Data.Rows.Count == 0)
                {
                    return;
                }
                for (int i = 0; i < DT_Data.Rows.Count; i++)
                {
                    IMEIenCurso = DT_Data.Rows[i]["IMEI"].ToString();

                    using (var client = new HttpClient())
                    {
                        //setup client
                        client.BaseAddress = new Uri("http://plataforma.soltrack.com");
                        client.DefaultRequestHeaders.Accept.Clear();
                        

                        var securityToken = "d9de38b6-fd06-4b17-85a9-cef35dc44fb2";
                        var from = DateTime.UtcNow.AddMinutes(-30).ToString("yyyy-MM-dd HH:mm");
                        var to = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm");

                        //send request
                        HttpResponseMessage responseMessage = await client.GetAsync("/WS/WSTrack2.asmx/GetPositionsByIMEIAndDateRange?securityToken=" + securityToken + "&imei=" + IMEIenCurso + "&from=" + from + "&to=" + to);
                       
                        //get access token from response body
                        responseJson = await responseMessage.Content.ReadAsStringAsync();



                        responseJson = System.Text.RegularExpressions.Regex.Replace(responseJson, "&lt;", "<");
                        responseJson = System.Text.RegularExpressions.Regex.Replace(responseJson, "&gt;", ">");

                        System.Xml.XmlDocument doc2 = new System.Xml.XmlDocument();
                        doc2.LoadXml(responseJson);
                        string json2 = JsonConvert.SerializeXmlNode(doc2);

                        json2 = System.Text.RegularExpressions.Regex.Replace(json2, "string", "Dataset");
                        json2 = json2.Replace("?", "");
                        json2 = json2.Replace("#", "");
                        json2 = System.Text.RegularExpressions.Regex.Replace(json2, "@", "");

                        if (json2 == "{\"xml\":{\"version\":\"1.0\",\"encoding\":\"utf-8\"},\"Dataset\":{\"xmlns\":\"http://www.tempuri.org/\",\"NewDataSet\":null}}")
                        {
                            continue;
                        }


                        var objetoresult= new Table();
                        try
                        {
                            var result = JsonConvert.DeserializeObject<ObjectResult>(json2);
                            objetoresult = result.Dataset.NewDataSet.Table;
                        }
                        catch (Exception Ex)
                        {
                            try
                            {
                                json2 = System.Text.RegularExpressions.Regex.Replace(json2, "Table", "Tables");
                                var result = JsonConvert.DeserializeObject<ObjectResult>(json2);
                                objetoresult = result.Dataset.NewDataSet.Tables[0];
                            }
                            catch (Exception Ex2)
                            {
                                continue;
                            }
                        }

                        if (objetoresult.IMEI == null || objetoresult.IMEI == "")
                        {
                            continue;
                        }
                        
                            //Consulta si ya existe la posicion, por si es repetida y no ha actualizado el equipo
                            string imei = objetoresult.IMEI.ToString();
                            string codigoevento = objetoresult.EventName == null ? "" : objetoresult.EventName.ToString();

                            string lat = objetoresult.Lat.ToString();
                            string lng = objetoresult.Lon.ToString();
                            //string evento = objetoresult.status.ToString();
                            string odometro = objetoresult.Odometer.ToString().Split('.')[0];
                            ////string placas = ((dynamic)objetoresult).Plates;
                            string velocidad = objetoresult.Speed.ToString().Split('.')[0];
                            string bateria = "0";
                            string direccion = objetoresult.Direction.ToString().Split('.')[0];
                        //2017 / 07 / 07 20:37:06
                            var fechahoragps = objetoresult.ActualDate;//YA VIENE EN UTC
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

                            WS_GPS_InsertaSimple = db.WS_GPS_InsertaSimple(UsuarioReC, imei, codigoevento, LAT, LNG, "", true, VELOCIDAD, DIRECCION, BATERIA, ODOMETRO, fechahoragps, fechahoragps).ToList();
                            if (WS_GPS_InsertaSimple[0].Indicador == 1)
                            {

                            }
                            else
                            {
                                log.Error("Error al Insertar evento de " + UsuarioReC + ". " + " IMEI: " + imei + ". " + WS_GPS_InsertaSimple[0].Mensaje + ". ");
                            }
                        


                    }
                }
            }
            catch (Exception Ex)
            {
                
                log.Error("Error Soltrack_ObtenerPosicion: " + UsuarioReC + ". " + responseJson + ". " + Ex.Message);
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


        public class Xml
        {
            public string version { get; set; }
            public string encoding { get; set; }
        }

        public class Table
        {
            public string PositionGUID { get; set; }
            public string PositionID { get; set; }
            public string MessageID { get; set; }
            public string SessionID { get; set; }
            public string ClientID { get; set; }
            public string GroupName { get; set; }
            public string ItemName { get; set; }
            public string IMEI { get; set; }
            public string ItemID { get; set; }
            public string Lat { get; set; }
            public string Lon { get; set; }
            public string Alt { get; set; }
            public string Speed { get; set; }
            public string Odometer { get; set; }
            public string EventName { get; set; }
            public DateTime ActualDate { get; set; }
            public string IsPoll { get; set; }
            public DateTime Created { get; set; }
            public string ActualDateUTC { get; set; }
            public string IgnitionStatus { get; set; }
            public string Direction { get; set; }
        }

        public class NewDataSet
        {
            public Table Table { get; set; }
            public IList<Table> Tables { get; set; }
        }

        public class Dataset
        {
            public string xmlns { get; set; }
            public NewDataSet NewDataSet { get; set; }
        }

        public class ObjectResult
        {
            public Xml xml { get; set; }
            public Dataset Dataset { get; set; }
        }


    }


}