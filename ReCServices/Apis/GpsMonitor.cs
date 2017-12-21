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
    public class GpsMonitor
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static async void GGpsMonitor_ObtenerPosicion(string UsuarioReC)
        {
            var responseJson = "";

            //Ejemplo de datos que retorna el WS
            //[{"odometer":37781247,"IMEI":"011892001916110","latitude":19.41855,"longitude":-99.0552833333,"speed":0,"course":0,"pos_date":"2017-04-06 18:54:28","pos_rec":"2017-11-07 22:23:13"},{"odometer":0,"IMEI":"865733028634491","latitude":19.4186066,"longitude":-99.055065,"speed":0,"course":336,"pos_date":"2017-10-15 01:01:36","pos_rec":"2017-11-07 22:23:13"},

            try
            {
                using (var client = new HttpClient())
                {
                    //setup client
                    client.BaseAddress = new Uri("https://goldenm.solutions");
                    client.DefaultRequestHeaders.Accept.Clear();
                   
                    //send request
                    HttpResponseMessage responseMessage = await client.GetAsync("/webservices/gpsmonitor4/");
                    
                    //get access token from response body
                    responseJson = await responseMessage.Content.ReadAsStringAsync();

                    //----<list<RootObject>>(*)----
                    var result = JsonConvert.DeserializeObject<List<RootObject>>(responseJson);

                    for (int i = 0; i < result.Count; i++)
                    {                        
                        try
                        {
                            //Consulta si ya existe la posicion, por si es repetida y no ha actualizado el equipo
                            string imei = result[i].IMEI.ToString();                            
                            string lat = result[i].latitude.ToString();
                            string lng = result[i].longitude.ToString();
                            string codigoevento = "0";
                            string odometro = result[i].odometer.ToString();                            
                            string velocidad = result[i].speed.ToString();
                            string bateria = "100";
                            string direccion = result[i].course.ToString().Split('.')[0];

                            var fechahoragps = DateTime.ParseExact(result[i].pos_date, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                            //fechahoragps = fechahoragps.ToUniversalTime();

                            var fechahoraserver = DateTime.ParseExact(result[i].pos_rec, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                            //fechahoraserver = fechahoraserver.ToUniversalTime();
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

                            WS_GPS_InsertaSimple = db.WS_GPS_InsertaSimple(UsuarioReC, imei, codigoevento, LAT, LNG, "", true, VELOCIDAD, DIRECCION, BATERIA, ODOMETRO, fechahoragps, fechahoraserver).ToList();
                            if (WS_GPS_InsertaSimple[0].Indicador == 1)
                            {

                            }
                            else
                            {
                                log.Error("Error al Insertar evento de " + UsuarioReC +". IMEI: " + imei + " " +  WS_GPS_InsertaSimple[0].Mensaje);
                            }
                        }
                        catch (Exception Ex)
                        {
                            log.Error("Error RedGPS_ObtenerPosicion: " + UsuarioReC + Ex.Message);
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
                    log.Error("Error GrupoGCP_ObtenerPosicion: " + UsuarioReC + ". " + responseJson + ". " + Ex.Message);
                }
            }
        }

        public class RootObject
        {
            public double odometer { get; set; }
            public string IMEI { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public int speed { get; set; }
            public int course { get; set; }
            public string pos_date { get; set; }
            public string pos_rec { get; set; }
        }
    }
}