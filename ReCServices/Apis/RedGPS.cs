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
    public static class RedGPS
    {


        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static string Token { get; set; }

        public static async void RedGPS_ObtenerToken(string UsuarioReC)
        {
            var responseJson = "";
            try
            {
                using (var client = new HttpClient())
                {
                    //setup client
                    client.BaseAddress = new Uri("http://api.redgps.com");
                    client.DefaultRequestHeaders.Accept.Clear();

                    //setup login data
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                    //new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("apikey", "3cc7690df44e947d330963e65528e34b"),
                    new KeyValuePair<string, string>("token", ""),
                    new KeyValuePair<string, string>("username", "panalpina"),
                    new KeyValuePair<string, string>("password", "transportesibarra")
                });

                    //send request
                    HttpResponseMessage responseMessage = await client.PostAsync("/api/v1/gettoken", formContent);
                    //HttpResponseMessage responseMessage = await client.PostAsync("/web%20services/ws_last_position/ws_last_position.asmx/GetLastPosition_02?User=" + Usuario  + "&Password=" + Password + "&Page=", formContent);

                    //get access token from response body
                    responseJson = await responseMessage.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<LoginObject>(responseJson);

                    Token = result.data;
                    log.Info("WebService RedGPS Autenticado: " + UsuarioReC + ". ");
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
                    log.Error("Error RedGPS_ObtenerToken: " + UsuarioReC + ". " + responseJson + ". " + Ex.Message);
                }
                Token = "";
            }
        }
        public static async void RedGPS_ObtenerPosicion(string UsuarioReC, string Usuario, string Password)
        {
            var responseJson = "";
            try
            {
                if (Token == null || Token == "")
                {
                    RedGPS_ObtenerToken(UsuarioReC);
                    return;
                }

                using (var client = new HttpClient())
                {
                    //setup client
                    client.BaseAddress = new Uri("http://api.redgps.com");
                    client.DefaultRequestHeaders.Accept.Clear();
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //setup login data
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("apikey", "3cc7690df44e947d330963e65528e34b"),
                    new KeyValuePair<string, string>("token", Token)
                    });

                    //send request
                    HttpResponseMessage responseMessage = await client.PostAsync("/api/v1/getdata", formContent);

                    //get access token from response body
                    responseJson = await responseMessage.Content.ReadAsStringAsync();

                    if (responseJson.Contains("error"))
                    {
                        Token = "";
                        RedGPS_ObtenerToken(UsuarioReC);
                        return;
                    }

                    var res = JsonConvert.DeserializeObject<ResultObject>(responseJson);

                    if (res.status == 30400 || res.data == null || res.data.Count == 0)
                    {
                        RedGPS_ObtenerToken(UsuarioReC);
                        return;
                    }

                    var result = res.data;

                    for (int i = 0; i < result.Count; i++)
                    {
                        //Consulta si ya existe la posicion, por si es repetida y no ha actualizado el equipo
                        string imei = result[i].GpsIdentif.ToString();
                        string codigoevento = "1";

                        string lat = result[i].Latitude.ToString();
                        string lng = result[i].Longitude.ToString();
                        //string evento = result[i].status.ToString();
                        string odometro = result[i].Odometer.ToString();
                        ////string placas = ((dynamic)res[i]).Plates;
                        string velocidad = result[i].GpsSpeed.ToString();
                        string bateria = "100";
                        string direccion = "0";

                        var fechahoragps = DateTime.ParseExact(result[i].ReportDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
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

                        WS_CONTEXT db = new WS_CONTEXT("WS_CONTEXT_PROD");

                        WS_GPS_InsertaSimple = db.WS_GPS_InsertaSimple(UsuarioReC, imei, codigoevento, LAT, LNG, "", true, VELOCIDAD, DIRECCION, BATERIA, ODOMETRO, fechahoragps, fechahoragps).ToList();
                        if (WS_GPS_InsertaSimple[0].Indicador == 1)
                        {

                        }
                        else
                        {
                            log.Error("Error al Insertar evento de " + UsuarioReC + ". " + WS_GPS_InsertaSimple[0].Mensaje + ". " + responseJson);
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
                    log.Error("Error RedGPS_ObtenerPosicion: " + UsuarioReC + ". " + responseJson + ". " + Ex.Message);
                }
            }
        }


        public class LoginObject
        {
            public int status { get; set; }
            public string data { get; set; }
        }

        public class Datum
        {
            public string UnitId { get; set; }
            public string UnitPlate { get; set; }
            public string GpsIdentif { get; set; }
            public string ReportDate { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public string Altitude { get; set; }
            public string GpsSpeed { get; set; }
            public string Direction { get; set; }
            public string Satellites { get; set; }
            public string Ignition { get; set; }
            public int Odometer { get; set; }
        }

        public class ResultObject
        {
            public int status { get; set; }
            public IList<Datum> data { get; set; }
        }
    }



    

}
