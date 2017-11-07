using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Dynamic;
//using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Globalization;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace ReCServices.Apis
{
    public static class Cybermapa
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static async void Cybermapa_ObtenerPosicion(string UsuarioReC, string Usuario, string Password)
        {

            var responseJson = "";
            
            try
            {
                

                using (var client = new HttpClient())
                {
                    //setup client
                    client.BaseAddress = new Uri("http://cloud13.cybermapa.com");
                    client.DefaultRequestHeaders.Accept.Clear();

                    var Content = new StringContent("{\"user\":\"" + Usuario + "\",\"pwd\":\"" + Password + "\",\"action\":\"DATOSACTUALES\"}", System.Text.Encoding.UTF8, "application/json");

                    //send request
                    HttpResponseMessage responseMessage = await client.PostAsync("/ws/ws.js", Content);

                    //get access token from response body
                    responseJson = await responseMessage.Content.ReadAsStringAsync();                    
                    
                    var result = JsonConvert.DeserializeObject<List<ObjectResult>>(responseJson);

                    for (int i = 0; i < result.Count; i++)
                    {
                        var imei = "";
                        try
                        {
                            //Consulta si ya existe la posicion, por si es repetida y no ha actualizado el equipo
                            imei = result[i].gps.ToString();
                            string codigoevento = result[i].evento.ToString();

                            string lat = result[i].latitud.ToString();
                            string lng = result[i].longitud.ToString();
                            //string evento = result[i].status.ToString();
                            string odometro = "0";
                            ////string placas = ((dynamic)res[i]).Plates;
                            string velocidad = result[i].velocidad.ToString();
                            string bateria = "100";
                            string direccion = result[i].sentido.ToString().Split('.')[0];

                            var fechahoragps = DateTime.ParseExact(result[i].fecha, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
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
                                log.Error("Error al Insertar evento de " + UsuarioReC + ". IMEI: " + imei + "  " + WS_GPS_InsertaSimple[0].Mensaje + ". " + responseJson);
                            }
                        }
                        catch (Exception Ex)
                        {
                           log.Error("Error RedGPS_ObtenerPosicion: " + UsuarioReC + ". " + imei + ". " + Ex.Message);
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
                    log.Error("Error RedGPS_ObtenerPosicion: " + UsuarioReC + ". " + Ex.Message);
                    //log.Error("Error RedGPS_ObtenerPosicion: " + UsuarioReC + ". " + responseJson + ". " + Ex.Message);
                }
            }


            
        }

        public class ObjectResult
        {
            public string nombre { get; set; }
            public string alias { get; set; }
            public string patente { get; set; }
            public string gps { get; set; }
            public string latitud { get; set; }
            public string longitud { get; set; }
            public string fecha { get; set; }
            public string sentido { get; set; }
            public string velocidad { get; set; }
            public string evento { get; set; }
        }
    }


}