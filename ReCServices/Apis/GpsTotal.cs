using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace ReCServices.Apis
{
    public static class GpsTotal
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static async void GpsTotal_ObtenerPosicion(string UsuarioReC, string Usuario, string Password)
        {
            var responseJson = "";
            try
            {
                using (var client = new HttpClient())
                {
                    //setup client
                    client.BaseAddress = new Uri("http://web.gpstotal.com.mx");
                    //client.DefaultRequestHeaders.Add("SOAPAction", "http://foo.com/GetVersion")
                    client.DefaultRequestHeaders.Accept.Clear();

                    //setup login data
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("SecurityToken", Password),
                    new KeyValuePair<string, string>("ClientName", Usuario)
                });

                    //send request
                    HttpResponseMessage responseMessage = await client.PostAsync("/WS/WSTrack2.asmx/GetCurrentPositionByClientNameWithAddress", formContent);
                    //HttpResponseMessage responseMessage = await client.PostAsync("/web%20services/ws_last_position/ws_last_position.asmx/GetLastPosition_02?User=" + Usuario  + "&Password=" + Password + "&Page=", formContent);

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

                    //json2 = json2.Replace("\"Plate\":[", "\"Units\":[");
                    //json2 = json2.Replace("cdata-section", "cdatasection");
                    var result = JsonConvert.DeserializeObject<RootObject>(json2);
                    //var res = result.dataset.NewDataSet.Table;


                    var res = result.Dataset.NewDataSet.Table;
                    //---


                    for (int i = 0; i < res.Count; i++)
                    {
                        //Consulta si ya existe la posicion, por si es repetida y no ha actualizado el equipo
                        string imei = res[i].ItemID.ToString();
                        string codigoevento = "";

                        string lat = res[i].Lat.ToString();
                        string lng = res[i].Lon.ToString();
                        //string evento = res[i].status.ToString();
                        string odometro = "0";
                        ////string placas = ((dynamic)res[i]).Plates;
                        string velocidad = res[i].Speed.ToString().Split('.')[0];
                        string bateria = "0";
                        string direccion = res[i].Direction.ToString().Split('.')[0];
                        //2017 / 07 / 07 20:37:06
                        var fechahoragps = res[i].Created;
                        fechahoragps = fechahoragps.ToUniversalTime();

                        var fechahoraserver = res[i].Created;
                        fechahoraserver = fechahoraserver.ToUniversalTime();
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

                        WS_CONTEXT db = new WS_CONTEXT("WS_CONTEXT_PROD");

                        WS_GPS_InsertaSimple = db.WS_GPS_InsertaSimple(UsuarioReC, imei, codigoevento, LAT, LNG, "", true, VELOCIDAD, DIRECCION, BATERIA, ODOMETRO, fechahoragps, fechahoraserver).ToList();
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
            public string ItemID { get; set; }
            public string Lat { get; set; }
            public string Lon { get; set; }
            public string Alt { get; set; }
            public string Speed { get; set; }
            public DateTime ActualDate { get; set; }
            public string IsPoll { get; set; }
            public DateTime Created { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string ZipCode { get; set; }
            public string Country { get; set; }
            public string ActualDateUTC { get; set; }
            public string IgnitionStatus { get; set; }
            public string Direction { get; set; }
        }

        public class NewDataSet
        {
            public List<Table> Table { get; set; }
        }

        public class Dataset
        {
            public string xmlns { get; set; }
            public NewDataSet NewDataSet { get; set; }
        }

        public class RootObject
        {
            public Xml xml { get; set; }
            public Dataset Dataset { get; set; }
        }








    }
}