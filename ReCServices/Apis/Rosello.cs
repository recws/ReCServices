using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace ReCServices.Apis
{
    public class Rosello
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static async void Rosello_ObtenerPosicion(string UsuarioReC, string Usuario, string Password)
        {
            var responseJson = "";
            try
            {
                using (var client = new HttpClient())
                {
                    //setup client
                    client.BaseAddress = new Uri("http://207.158.15.160:20999");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                    //send request
                    HttpResponseMessage responseMessage = await client.GetAsync("/positionv1/" + Password);
                    //HttpResponseMessage responseMessage = await client.PostAsync("/web%20services/ws_last_position/ws_last_position.asmx/GetLastPosition_02?User=" + Usuario  + "&Password=" + Password + "&Page=", formContent);

                    //get access token from response body
                    responseJson = await responseMessage.Content.ReadAsStringAsync();
                                        
                    var res = JsonConvert.DeserializeObject<RootObject>(responseJson);

                    var result = res.position;

                    for (int i = 0; i < result.Count; i++)
                    {
                        try
                        {
                            string imei = result[i].imei.ToString();
                            string codigoevento = "";

                            string lat = result[i].lat.ToString();
                            string lng = result[i].lng.ToString();
                            //string evento = result[i].status.ToString();
                            string odometro = result[i].odometer.ToString();
                            ////string placas = ((dynamic)res[i]).Plates;
                            string velocidad = result[i].speed.ToString();
                            string bateria = result[i].battery.ToString();
                            string direccion = result[i].course.ToString();

                            DateTime fechahoragps = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(result[i].gmt);
                            //fechahoragps = fechahoragps.ToUniversalTime();
                            DateTime fechahoraserver = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(result[i].last_rep);
                            //fechahoraserver = fechahoraserver.ToUniversalTime();
                            //DateTime fechahoragps = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(result.items[i].pos.t);

                            ////Conversiones de datos
                            //codigoevento = codigoevento;
                            //evento = evento;
                            var LAT = decimal.Parse(lat);
                            var LNG = decimal.Parse(lng);
                            var ODOMETRO = int.Parse(odometro);
                            //var PLACAS = System.Text.RegularExpressions.Regex.Replace(placas, "-", "");
                            //PLACAS = System.Text.RegularExpressions.Regex.Replace(PLACAS, " ", "");
                            var VELOCIDAD = int.Parse(velocidad);
                            var DIRECCION = int.Parse(direccion);
                            var BATERIA = int.Parse(bateria);
                            //string[] formats = { "M/dd/yyyy hh:mm:ss tt" };
                            //var dateTime = DateTime.ParseExact(fechahora, formats, new System.Globalization.CultureInfo("en-US"), System.Globalization.DateTimeStyles.None);
                            
                            //Si no es repetida la inserta
                            List<WS_GPS_InsertaSimple_Result> WS_GPS_InsertaSimple;

                            WS_CONTEXT db = new WS_CONTEXT();

                            WS_GPS_InsertaSimple = db.WS_GPS_InsertaSimple(UsuarioReC, imei, codigoevento, LAT, LNG, "", true, VELOCIDAD, DIRECCION, BATERIA, ODOMETRO, fechahoragps, fechahoraserver).ToList();
                            if (WS_GPS_InsertaSimple[0].Indicador == 1)
                            {

                            }
                            else
                            {
                                var json = JsonConvert.SerializeObject(result[i]);
                                log.Error("Error al Insertar evento de " + UsuarioReC + ". IMEI: " + imei + ". " + WS_GPS_InsertaSimple[0].Mensaje + ". " + result[i]);
                            }
                        }
                        catch (Exception Ex)
                        {
                            log.Error("Error Rosello_ObtenerPosicion: " + UsuarioReC + ". " + Ex.Message);
                            continue;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {   
                log.Error("Error Rosello_ObtenerPosicion: " + UsuarioReC + ". " + responseJson + ". " + Ex.Message);                
            }

        }

        public class Position
        {
            public object imei { get; set; }
            public int gmt { get; set; }
            public double lat { get; set; }
            public double lng { get; set; }
            public int course { get; set; }
            public int speed { get; set; }
            public int odometer { get; set; }
            public int run { get; set; }
            public int door { get; set; }
            public int battery { get; set; }
            public int tpr { get; set; }
            public int last_rep { get; set; }
        }

        public class RootObject
        {
            public List<Position> position { get; set; }
        }

    }
}