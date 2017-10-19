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

namespace ReCServices.Apis
{
    public static class Boson
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static async void BOSON_ObtenerPosicion(string UsuarioReC, string Usuario, string Password)
        {
            var responseJson = "";
            try
            {
                using (var client = new HttpClient())
                {
                    //setup client
                    client.BaseAddress = new Uri("http://www.utrax2.com");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //setup login data
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                    //new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("User", Usuario),
                    new KeyValuePair<string, string>("Password", Password),
                    new KeyValuePair<string, string>("Page", ""),
                });

                    //send request
                    HttpResponseMessage responseMessage = await client.PostAsync("/web%20services/ws_last_position/ws_last_position.asmx/GetLastPosition_02", formContent);
                    //HttpResponseMessage responseMessage = await client.PostAsync("/web%20services/ws_last_position/ws_last_position.asmx/GetLastPosition_02?User=" + Usuario  + "&Password=" + Password + "&Page=", formContent);

                    //get access token from response body
                    responseJson = await responseMessage.Content.ReadAsStringAsync();

                    //log.Info("Data BOSON_ObtenerPosicion: " + UsuarioReC + ". " + responseJson.ToString());
                    //load into XElement
                    XElement doc = XElement.Parse(responseJson);
                    var res = doc.ToDynamicList();

                    for (int i = 0; i < res.Count; i++)
                    {
                        if (((dynamic)res[i]).Device_Id == "~" || ((dynamic)res[i]).Device_Id == "1" || ((System.Dynamic.ExpandoObject)(res[i])).Count() <= 5)
                        {
                            continue;
                        }
                        //Consulta si ya existe la posicion, por si es repetida y no ha actualizado el equipo
                        string imei = ((dynamic)res[i]).Device_Id;
                        string codigoevento = ((dynamic)res[i]).eCode;

                        string lat = ((dynamic)res[i]).Latitude;
                        string lng = ((dynamic)res[i]).Longitude;
                        string evento = ((dynamic)res[i]).Evento;
                        string odometro = ((dynamic)res[i]).Odometer;
                        string placas = ((dynamic)res[i]).Plates;
                        string velocidad = ((dynamic)res[i]).Speed;
                        string direccion = ((dynamic)res[i]).Course;
                        string fechahora = ((dynamic)res[i]).DateTime_GPS;

                        //Validaciones
                        if (imei == "~" || imei.Length <= 1)
                        {
                            continue;
                        }
                        if (lng == "~" || lng.Length <= 1)
                        {
                            continue;
                        }
                        if (lat == "~" || lat.Length <= 1)
                        {
                            continue;
                        }
                        if (fechahora == "~" || fechahora.Length <= 1)
                        {
                            continue;
                        }

                        imei = imei == "~" ? "" : imei;
                        codigoevento = codigoevento == "~" ? "" : codigoevento;
                        evento = evento == "~" ? "" : evento;
                        lat = lat == "~" ? "0" : lat;
                        lng = lng == "~" ? "0" : lng;
                        odometro = odometro == "~" ? "0" : odometro;
                        placas = placas == "~" ? "" : placas;

                        velocidad = velocidad == "~" ? "0" : velocidad;
                        if (velocidad.IndexOf(".") >= 0)
                        {
                            velocidad = velocidad.Remove(velocidad.IndexOf("."));
                        }
                        direccion = direccion == "~" ? "0" : direccion;
                        if (direccion.IndexOf(".") >= 0)
                        {
                            direccion = direccion.Remove(direccion.IndexOf("."));
                        }
                        fechahora = fechahora == "~" ? "1900-01-01" : fechahora;

                        ////Conversiones de datos
                        //codigoevento = codigoevento;
                        //evento = evento;
                        var LAT = decimal.Parse(lat);
                        var LNG = decimal.Parse(lng);
                        var ODOMETRO = int.Parse(odometro);
                        var PLACAS = System.Text.RegularExpressions.Regex.Replace(placas, "-", "");
                        PLACAS = System.Text.RegularExpressions.Regex.Replace(PLACAS, " ", "");
                        var VELOCIDAD = int.Parse(velocidad);
                        var DIRECCION = int.Parse(direccion);
                        //string[] formats = { "M/dd/yyyy hh:mm:ss tt" };
                        //var dateTime = DateTime.ParseExact(fechahora, formats, new System.Globalization.CultureInfo("en-US"), System.Globalization.DateTimeStyles.None);
                        var dateTime = DateTime.ParseExact(fechahora, "M/d/yyyy h:m:ss tt", CultureInfo.InvariantCulture);
                        dateTime = dateTime.ToUniversalTime();

                        //Si no es repetida la inserta
                        List<WS_GPS_InsertaSimple_Result> WS_GPS_InsertaSimple;

                        WS_CONTEXT db = new WS_CONTEXT("WS_CONTEXT_PROD");

                        WS_GPS_InsertaSimple = db.WS_GPS_InsertaSimple(UsuarioReC, imei, codigoevento, decimal.Parse(lat), decimal.Parse(lng), "", true, int.Parse(velocidad), int.Parse(direccion), 100, int.Parse(odometro), dateTime, dateTime).ToList();
                        if (WS_GPS_InsertaSimple[0].Indicador == 1)
                        {

                        }
                        else
                        {
                            var json = JsonConvert.SerializeObject(res[i]);
                            log.Error("Error al Insertar evento de " + UsuarioReC + ". " + WS_GPS_InsertaSimple[0].Mensaje + ". " + json);
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
                if (Ex.InnerException != null && Ex.InnerException.Message == "Unable to connect to the remote server")
                {
                    //El servidor Boson no esta disponible o bien no esta nuestra ip autorizada en su servidor
                    log.Error("Error BOSON_ObtenerPosicion: " + UsuarioReC + ". " + responseJson + ". " + Ex.InnerException.Message + ". " + " Verificar que la ip este autorizada con Boson.");
                }
                else if (Ex.InnerException != null)
                {
                    log.Error("Error BOSON_ObtenerPosicion: " + UsuarioReC + ". " + responseJson + ". " + Ex.InnerException.Message);
                    //No hay conexion al servidor, a veces se protege de conexiones frecuentes., pero despues si deja conectarse.
                }
                else
                {
                    log.Error("Error BOSON_ObtenerPosicion: " + UsuarioReC + ". " + responseJson + ". " + Ex.Message);
                }
            }
        }
    }
}