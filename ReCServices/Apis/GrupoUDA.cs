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
    public static class GrupoUDA
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static async void GrupoUDA_ObtenerPosicion(string UsuarioReC, string Usuario, string Password)
        {
            var responseJson = "";
            try
            {
                using (var client = new HttpClient())
                {
                    //setup client
                    client.BaseAddress = new Uri("http://ws.grupouda.com.mx");
                    client.DefaultRequestHeaders.Accept.Clear();
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //setup login data
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                    //new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("sLogin", Usuario),
                    new KeyValuePair<string, string>("sPassword", Password)
                });

                    //send request
                    HttpResponseMessage responseMessage = await client.PostAsync("/wsUDAHistoryGetByPlate.asmx/HistoyDataLastLocationByUser", formContent);
                    //HttpResponseMessage responseMessage = await client.PostAsync("/web%20services/ws_last_position/ws_last_position.asmx/GetLastPosition_02?User=" + Usuario  + "&Password=" + Password + "&Page=", formContent);

                    //get access token from response body
                    responseJson = await responseMessage.Content.ReadAsStringAsync();


                    //responseJson = System.Text.RegularExpressions.Regex.Replace(responseJson, "&lt;", "<");
                    //responseJson = System.Text.RegularExpressions.Regex.Replace(responseJson, "&gt;", ">");

                    System.Xml.XmlDocument doc2 = new System.Xml.XmlDocument();
                    doc2.LoadXml(responseJson);
                    string json2 = JsonConvert.SerializeXmlNode(doc2);

                    json2 = System.Text.RegularExpressions.Regex.Replace(json2, "string", "Dataset");
                    json2 = json2.Replace("?", "");
                    json2 = json2.Replace("#", "");
                    json2 = System.Text.RegularExpressions.Regex.Replace(json2, "@", "");
                    
                    json2 = json2.Replace("\"Plate\":[", "\"Units\":[");
                    json2 = json2.Replace("cdata-section", "cdatasection");

                    var result = JsonConvert.DeserializeObject<ObjectResult>(json2);
                    //var res = result.dataset.NewDataSet.Table;


                    var res = result.space.Response.Units;

                    for (int i = 0; i < res.Count; i++)
                    {
                        //Consulta si ya existe la posicion, por si es repetida y no ha actualizado el equipo
                        string imei = res[i].hst.Imei.cdatasection.ToString();
                        string codigoevento = res[i].hst.EventID.ToString() == null ? "" : res[i].hst.EventID.ToString();

                        string lat = res[i].hst.Latitude.ToString();
                        string lng = res[i].hst.Longitude.ToString();
                        //string evento = res[i].status.ToString();
                        string odometro = res[i].hst.DefaultOdometer.ToString().Split('.')[0];
                        ////string placas = ((dynamic)res[i]).Plates;
                        string velocidad = res[i].hst.Speed.ToString().Split('.')[0];
                        string bateria = "0";
                        string direccion = res[i].hst.Angle.ToString();
                        //2017 / 07 / 07 20:37:06
                        var fechahoragps = DateTime.ParseExact(res[i].hst.DateTimeGPS, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
                        fechahoragps = fechahoragps.ToUniversalTime();
                        var fechahoraserver = DateTime.ParseExact(res[i].hst.DateTimeServer, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
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
                            log.Error("Error al Insertar evento de " + UsuarioReC + ". " + " IMEI: " + imei + ". " + WS_GPS_InsertaSimple[0].Mensaje + ". " );
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

        public class Status
        {
            public string code { get; set; }
            public string description { get; set; }
        }

        public class Alias
        {
            public string cdatasection { get; set; }
        }

        public class ECO
        {
            public string cdatasection { get; set; }
        }

        public class Plate
        {
            public string cdatasection { get; set; }
            }

            public class Location
        {
            public string cdatasection { get; set; }
            }

            public class Event
        {
            public string cdatasection { get; set; }
            }

            public class Imei
        {
            public string cdatasection { get; set; }
            }

            public class SimCardPhone
        {
            public string cdatasection { get; set; }
            }

            public class CustomerPass
        {
            public string cdatasection { get; set; }
            }

            public class Hst
        {
            public string id { get; set; }
            public Alias Alias { get; set; }
            public ECO ECO { get; set; }
            public Units Units { get; set; }
            public string DateTimeGPS { get; set; }
            public string DateTimeServer { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public string Speed { get; set; }
            public string Heading { get; set; }
            public string Angle { get; set; }
            public Location Location { get; set; }
            public string EventID { get; set; }
            public Event Event { get; set; }
            public string IP { get; set; }
            public Imei Imei { get; set; }
            public string IgnitionState { get; set; }
            public string Fleet { get; set; }
            public string TypeMobile { get; set; }
            public string Priority { get; set; }
            public SimCardPhone SimCardPhone { get; set; }
            public string GpsFix { get; set; }
            public CustomerPass CustomerPass { get; set; }
            public string DeviceID { get; set; }
            public string DeviceName { get; set; }
            public string DeviceDesc { get; set; }
            public string DefaultOdometer { get; set; }
        }

        public class Units
        {
            public string id { get; set; }
            public string uid { get; set; }
            public Hst hst { get; set; }
        }

        public class Response
        {
            public Status Status { get; set; }
            public IList<Units> Units { get; set; }
        }

        public class Space
        {
            public Response Response { get; set; }
        }

        public class ObjectResult
        {
            public Xml xml { get; set; }
            public Space space { get; set; }
        }
    }

    
}