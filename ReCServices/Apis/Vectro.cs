using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Globalization;

namespace ReCServices.Apis
{
    public static class Vectro
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static async void VECTRO_ObtenerPosicion(string UsuarioReC, string Usuario, string Password)
        {
            var responseJson = "";
            try
            {
                using (var client = new HttpClient())
                {
                    //setup client
                    client.BaseAddress = new Uri("http://162.248.55.111:8091");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    

                    //send request
                    HttpResponseMessage responseMessage = await client.GetAsync("/panalpina");
                    //HttpResponseMessage responseMessage = await client.PostAsync("/web%20services/ws_last_position/ws_last_position.asmx/GetLastPosition_02?User=" + Usuario  + "&Password=" + Password + "&Page=", formContent);

                    //get access token from response body
                    responseJson = await responseMessage.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<List<ObjectResult>>(responseJson);

                    
                    for (int i = 0; i < result.Count; i++)
                    {
                        try
                        {
                            if (result[i].SERIE == null || result[i].LAT == null || result[i].LON == null)
                            {
                                continue;
                            }

                            string imei = result[i].SERIE.ToString();
                            string codigoevento = result[i].IN.ToString();

                            string lat = result[i].LAT.ToString().Replace("+", "");
                            string lng = result[i].LON.ToString();
                            //string evento = result[i].status.ToString();
                            string odometro = "0";
                            ////string placas = ((dynamic)res[i]).Plates;
                            string velocidad = result[i].SPD.ToString();
                            string bateria = result[i].BAT == null ? "0" : result[i].BAT.ToString();
                            string direccion = "0";

                            var fechahoragps = DateTime.ParseExact(result[i].TIMEIN, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                            fechahoragps = fechahoragps.ToUniversalTime();
                            var fechahoraserver = DateTime.ParseExact(result[i].TIMEOUT, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                            fechahoraserver = fechahoraserver.ToUniversalTime();

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
                            //string[] formats = { "M/dd/yyyy hh:mm:ss tt" };
                            //var dateTime = DateTime.ParseExact(fechahora, formats, new System.Globalization.CultureInfo("en-US"), System.Globalization.DateTimeStyles.None);


                            //Si no es repetida la inserta
                            List<WS_GPS_InsertaSimple_Result> WS_GPS_InsertaSimple;

                            WS_CONTEXT db = new WS_CONTEXT("WS_CONTEXT_PROD");

                            WS_GPS_InsertaSimple = db.WS_GPS_InsertaSimple(UsuarioReC, imei, codigoevento, decimal.Parse(lat), decimal.Parse(lng), "", true, int.Parse(velocidad), int.Parse(direccion), 100, int.Parse(odometro), fechahoragps, fechahoraserver).ToList();
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
                            log.Error("Error BOSON_ObtenerPosicion: " + UsuarioReC + ". " + responseJson + ". " + Ex.Message);
                            continue;
                        }
                    }

                }
            }
            catch (Exception Ex)
            {
                if (Ex.InnerException != null && Ex.InnerException.Message == "No es posible conectar con el servidor remoto")
                {
                    //No hay conexion al servidor, a veces se protege de conexiones frecuentes., pero despues si deja conectarse.
                }
                else
                {
                    log.Error("Error BOSON_ObtenerPosicion: " + UsuarioReC + ". " + responseJson + ". " + Ex.Message);
                }
            }
        }


        public class ObjectResult
        {
            public string SERIE { get; set; }
            public string PLACAS { get; set; }
            public string LAT { get; set; }
            public string LON { get; set; }
            public string SPD { get; set; }
            public string HGT { get; set; }
            public string IN { get; set; }
            public int? ENC { get; set; }
            public string TIMEIN { get; set; }
            public string TIMEOUT { get; set; }
            public string BAT { get; set; }
        }
    }


}