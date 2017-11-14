using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace ReCServices.Apis
{
    public class SolucionesGlobales
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static string Token { get; set; }

        public static async void SolucionesGloblaes_ObtenerToken(string UsuarioWS, string PasswordWS)
        {
            var responseJson = "";
            try
            {
                using (var client = new HttpClient())
                {
                    //setup client
                    client.BaseAddress = new Uri("http://162.243.146.161");
                    client.DefaultRequestHeaders.Accept.Clear();

                    //send request
                    HttpResponseMessage responseMessage = await client.GetAsync("/api/v1/sessions/new/?user_name=" + UsuarioWS + "&password=" + PasswordWS);

                    //get access token from response body
                    responseJson = await responseMessage.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<TokenObject>(responseJson);

                    Token = result.access_token;
                    log.Info("WebService RedGPS Autenticado: " + UsuarioWS + ". ");
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
                    log.Error("Error RedGPS_ObtenerToken: " + UsuarioWS + ". " + responseJson + ". " + Ex.Message);
                }
                Token = "";
            }
        }

        public static async void SolucionesGloblaes_ObtenerPosicion(string UsuarioReC, string Usuario, string Password)
        {

            //{"id":3198,"name":"PETERBILT 002","car_image_id":5,"current_temperature":null,"latitude":26.186633,"longitude":-97.968189,"engine_status":"off","last_event_type":"position","battery":null,"speed":0,"plates":null,"mileage":172395.3,"orientation":269,"licence_id":null,"geocoded_area":[{"id":63031992,"lat":26.186633,"long":-97.968189,"city":"Weslaco","street":"2611 Vo Tech Drive","reference":"Weslaco, TX, USA","distance":3.76796770975726,"created_at":"2017-11-13T23:55:48.085Z","updated_at":"2017-11-13T23:55:48.085Z","device_id":0}],"last_update":"2017-11-13T23:40:01.034Z"}

            var responseJson = "";
            try
            {
                if (Token == null || Token == "")
                {
                    SolucionesGloblaes_ObtenerToken(Usuario, Password);
                    //return;
                }

                using (var client = new HttpClient())
                {
                    //setup client
                    client.BaseAddress = new Uri("http://162.243.146.161");
                    client.DefaultRequestHeaders.Accept.Clear();


                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

                    //send request
                    HttpResponseMessage responseMessage = await client.GetAsync("/api/v2/map/");

                    //get access token from response body
                    responseJson = await responseMessage.Content.ReadAsStringAsync();

                    var res = JsonConvert.DeserializeObject<RootObject>(responseJson);

                    var result = res.values.groupless;

                    for (int i = 0; i < result.Count; i++)
                    {


                        //Consulta si ya existe la posicion, por si es repetida y no ha actualizado el equipo
                        string imei = result[i].id.ToString();
                        string codigoevento = "1";

                        string lat = result[i].latitude.ToString();
                        string lng = result[i].longitude.ToString();
                        //string evento = result[i].status.ToString();
                        string odometro = "0";
                        ////string placas = ((dynamic)res[i]).Plates;
                        string velocidad = result[i].speed.ToString();
                        string bateria = result[i].battery == null ? "0" : result[i].battery.ToString();
                        //string bateria = result.vehicle_states[i].Aux_Battery == null ? "0" : result.vehicle_states[i].Aux_Battery.ToString();
                        string direccion = result[i].orientation.ToString();

                        var fechahoragps = DateTime.ParseExact(result[i].last_update.ToString("dd-MM-yyyy HH:mm:ss"), "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
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

        //--------TOKEN------
        public class TokenObject
        {
            public string password_digest { get; set; }
            public string access_token { get; set; }
            public int id { get; set; }
            public string user_name { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public int user_id { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public object reg_id { get; set; }
            public string blue_point_user_token { get; set; }
            public string blue_point_user_id { get; set; }
            public bool support { get; set; }
            public bool sub_user { get; set; }
        }

        //---------LAT/LONG--------
        public class GeocodedArea
        {
            public int id { get; set; }
            public double lat { get; set; }
            public double @long { get; set; }
            public string city { get; set; }
            public string street { get; set; }
            public string reference { get; set; }
            public double distance { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public int device_id { get; set; }
        }

        public class Groupless
        {
            public int id { get; set; }
            public string name { get; set; }
            public int car_image_id { get; set; }
            public object current_temperature { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public string engine_status { get; set; }
            public string last_event_type { get; set; }
            public int? battery { get; set; }
            public double speed { get; set; }
            public object plates { get; set; }
            public double mileage { get; set; }
            public int orientation { get; set; }
            public object licence_id { get; set; }
            public List<GeocodedArea> geocoded_area { get; set; }
            public DateTime last_update { get; set; }
        }

        public class Values
        {
            public List<object> groups { get; set; }
            public List<Groupless> groupless { get; set; }
        }

        public class RootObject
        {
            public Values values { get; set; }
        }
    }
}