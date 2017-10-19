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
    public static class Quiala
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static async void QUIALA_ObtenerPosicion(string UsuarioReC, string Usuario, string Password)
        {
            var responseJson = "";
            try
            {
                using (var client = new HttpClient())
                {
                    //setup client
                    client.BaseAddress = new Uri("http://www.rastreo.blac.com.mx");
                    client.DefaultRequestHeaders.Accept.Clear();
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //setup login data
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                    //new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", Usuario),
                    new KeyValuePair<string, string>("password", Password)
                });

                    //send request
                    HttpResponseMessage responseMessage = await client.PostAsync("/WS/WSTrack2.asmx/GetVehiclesInformationBlacSolutions", formContent);
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
                    json2 = System.Text.RegularExpressions.Regex.Replace(json2, "@", "");


                    var result = JsonConvert.DeserializeObject<ObjectResult>(json2);
                    var res = result.dataset.NewDataSet.Table;

                    for (int i = 0; i < res.Count; i++)
                    {
                        try
                        {
                            //Consulta si ya existe la posicion, por si es repetida y no ha actualizado el equipo
                            string imei = res[i].IMEI.ToString();
                            string codigoevento = res[i].EventCode == null ? "" : res[i].EventCode.ToString();

                            string lat = res[i].Lat.ToString();
                            string lng = res[i].Lon.ToString();
                            //string evento = res[i].status.ToString();
                            string odometro = "0";
                            ////string placas = ((dynamic)res[i]).Plates;
                            string velocidad = res[i].Speed.ToString().Split('.')[0];
                            string bateria = "0";
                            string direccion = "0";

                            DateTime fechahoragps = res[i].Date.ToUniversalTime();

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

                            WS_GPS_InsertaSimple = db.WS_GPS_InsertaSimple(UsuarioReC, imei, codigoevento, LAT, LNG, "", true, VELOCIDAD, DIRECCION, BATERIA, ODOMETRO, fechahoragps, fechahoragps).ToList();
                            if (WS_GPS_InsertaSimple[0].Indicador == 1)
                            {

                            }
                            else
                            {
                                log.Error("Error al Insertar evento de " + UsuarioReC + ". " + WS_GPS_InsertaSimple[0].Mensaje + ". " + res[i].IMEI.ToString());
                            }
                        }
                        catch (Exception Ex)
                        {
                            log.Error("Error al obtener e Insertar evento de: " + UsuarioReC + ". " + res[i].IMEI.ToString() + ". " + Ex.Message);
                            continue;
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
                    log.Error("Error QUIALA_ObtenerPosicion: " + UsuarioReC + ". " + responseJson + ". " + Ex.Message);
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
            public string IMEI { get; set; }
            public string Lat { get; set; }
            public string Lon { get; set; }
            public DateTime Date { get; set; }
            public string Address { get; set; }
            public string Speed { get; set; }
            public string EventCode { get; set; }
            public string POIName { get; set; }
        }

        public class NewDataSet
        {
            public IList<Table> Table { get; set; }
        }

        public class Dataset
        {
            public string xmlns { get; set; }
            public NewDataSet NewDataSet { get; set; }
        }

        public class ObjectResult
        {
            public Xml xml { get; set; }
            public Dataset dataset { get; set; }
        }
    }

    
}