using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace ReCServices.Apis
{
    public class Sama
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static async void Sama_ObtenerPosicion(string UsuarioReC, string Usuario, string Password)
        {
            var responseJson = "";

            try
            {

                using (var client = new HttpClient())
                {
                    //setup client
                    client.BaseAddress = new Uri("http://149.56.244.50:8080");
                    client.DefaultRequestHeaders.Accept.Clear();

                    //send request
                    HttpResponseMessage responseMessage = await client.GetAsync("/events/data.jsonx?a=trapol&u=" + Usuario + "&p=" + Password + "&g=all&l=1");

                    //get access token from response body
                    responseJson = await responseMessage.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<RootObject>(responseJson);

                    var res = result.DeviceList;

                    for (int i = 0; i < res.Count; i++)
                    {
                        //Consulta si ya existe la posicion, por si es repetida y no ha actualizado el equipo
                        string imei = "000000";//Hecer query imei?  res[].EventData[0]. res[i].hst.Imei.cdatasection.ToString();
                        string codigoevento = res[i].EventData[0].StatusCode.ToString();

                        string lat = res[i].EventData[0].GPSPoint_lat.ToString();
                        string lng = res[i].EventData[0].GPSPoint_lon.ToString();
                        //string evento = res[i].status.ToString();
                        string odometro = res[i].EventData[0].Odometer.ToString().Split('.')[0];
                        ////string placas = ((dynamic)res[i]).Plates;
                        string velocidad = res[i].EventData[0].Speed.ToString().Split('.')[0];
                        string bateria = "0";
                        string direccion = res[i].EventData[0].Heading.ToString().Split('.')[0];
                        //2017 / 07 / 07 20:37:06
                        var fechahoragps = DateTime.ParseExact(res[i].EventData[0].Timestamp_date + " " + res[i].EventData[0].Timestamp_time, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
                        fechahoragps = fechahoragps.ToUniversalTime();
                        var fechahoraserver = DateTime.ParseExact(res[i].EventData[0].Timestamp_date + " " + res[i].EventData[0].Timestamp_time, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
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

        public class EventData
        {
            public string Device { get; set; }
            public int Timestamp { get; set; }
            public string Timestamp_date { get; set; }
            public string Timestamp_time { get; set; }
            public int IgnitionStatus { get; set; }
            public int StatusCode { get; set; }
            public string StatusCode_hex { get; set; }
            public string StatusCode_desc { get; set; }
            public string GPSPoint { get; set; }
            public double GPSPoint_lat { get; set; }
            public double GPSPoint_lon { get; set; }
            public double Speed_kph { get; set; }
            public double Speed { get; set; }
            public string Speed_units { get; set; }
            public double Heading { get; set; }
            public string Heading_desc { get; set; }
            public double Altitude_meters { get; set; }
            public int Altitude { get; set; }
            public string Altitude_units { get; set; }
            public double Odometer_km { get; set; }
            public double Odometer { get; set; }
            public string Odometer_units { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string PostalCode { get; set; }
            public int DigitalInputMask { get; set; }
            public string DigitalInputMask_hex { get; set; }
            public int Index { get; set; }
        }

        public class DeviceList
        {
            public string Device { get; set; }
            public string Device_desc { get; set; }
            public string Device_Plate { get; set; }
            public string Device_SimNumber { get; set; }
            public List<EventData> EventData { get; set; }
        }

        public class RootObject
        {
            public string Account { get; set; }
            public string Account_desc { get; set; }
            public string TimeZone { get; set; }
            public List<DeviceList> DeviceList { get; set; }
        }
    }
}