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
    public static class GrupoGCP
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static async void GrupoGCP_ObtenerPosicion(string UsuarioReC, string Usuario, string Password)
        {
            var responseJson = "";
            var IMEIenCurso = "";
            DataTable DT_Data = new DataTable();
            try
            {
                //Carga los IMEI que se van a consultar
                DT_Data = GetData_ListaGPSxProveedor("Grupo GCP");
                if (DT_Data.Rows.Count == 0)
                {
                    return;   
                }
                for (int i = 0; i < DT_Data.Rows.Count; i++)
                {
                    IMEIenCurso = DT_Data.Rows[i]["IMEI"].ToString();
                    using (var client = new HttpClient())
                    {
                        //setup client
                        client.BaseAddress = new Uri("http://gcp2.cechire.com:8081");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        //send request
                        HttpResponseMessage responseMessage = await client.GetAsync("/events/data.jsonx?a=gcp2020&p=panalpina&u=panalpina&d=" + IMEIenCurso + "&l=1&at=true");
                        //HttpResponseMessage responseMessage = await client.PostAsync("/web%20services/ws_last_position/ws_last_position.asmx/GetLastPosition_02?User=" + Usuario  + "&Password=" + Password + "&Page=", formContent);

                        //get access token from response body
                        responseJson = await responseMessage.Content.ReadAsStringAsync();

                        if (responseJson.Contains("Invalid device"))
                        {
                            log.Error("Error al Insertar evento de " + UsuarioReC + ". " + "IMEI: " + IMEIenCurso + ". " + responseJson);
                            continue;
                        }
                        else if (responseJson.Contains("Device(s) not authorized"))
                        {
                            log.Error(UsuarioReC + ". " + "IMEI: " + IMEIenCurso + ". " + "No Autorizado");
                            continue;
                        }
                        
                        var result = JsonConvert.DeserializeObject<ObjectResult>(responseJson);
                        var res = result.DeviceList[0].EventData[0];  //Esta consulta solo trae un dispositivo a la vez

                            //Consulta si ya existe la posicion, por si es repetida y no ha actualizado el equipo
                            string imei = res.Device.Replace("mvt_","").ToString();
                            string codigoevento = res.StatusCode.ToString() == null ? "" : res.StatusCode.ToString();

                            string lat = res.GPSPoint_lat.ToString();
                            string lng = res.GPSPoint_lon.ToString();
                            //string evento = res.status.ToString();
                            string odometro = res.Odometer.ToString().Split('.')[0];
                            ////string placas = ((dynamic)res).Plates;
                            string velocidad = res.Speed.ToString().Split('.')[0];
                            string bateria = "100";
                            string direccion = res.Heading.ToString().Split('.')[0];

                        DateTime fechahoragps = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(res.Timestamp);

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
                    log.Error("Error GrupoGCP_ObtenerPosicion: " + UsuarioReC + ". " + "IMEI: " + IMEIenCurso + ". " + responseJson + ". " + Ex.Message);
                }
            }
        }

        public static DataTable GetData_ListaGPSxProveedor(string Proveedor)
        {
            DataTable dtbl = new DataTable("DataTable1");

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WS_PROD"].ConnectionString);
            SqlDataAdapter adp = new SqlDataAdapter();
            adp.SelectCommand = new SqlCommand();
            adp.SelectCommand.Connection = con;

            adp.SelectCommand.CommandText = "WS_GPS_ListaGPSxProveedor";
            adp.SelectCommand.CommandType = CommandType.StoredProcedure;
            adp.SelectCommand.Parameters.AddWithValue("@IdTransportista", 0);
            adp.SelectCommand.Parameters.AddWithValue("@Proveedor", Proveedor);
            adp.Fill(dtbl);

            return dtbl;
        }


        public class EventData
        {
            public string Device { get; set; }
            public int Timestamp { get; set; }
            public string Timestamp_date { get; set; }
            public string Timestamp_time { get; set; }
            public int StatusCode { get; set; }
            public string StatusCode_hex { get; set; }
            public string StatusCode_desc { get; set; }
            public string GPSPoint { get; set; }
            public double GPSPoint_lat { get; set; }
            public double GPSPoint_lon { get; set; }
            public double Speed { get; set; }
            public string Speed_units { get; set; }
            public double Heading { get; set; }
            public string Heading_desc { get; set; }
            public int Altitude { get; set; }
            public string Altitude_units { get; set; }
            public double Odometer { get; set; }
            public string Odometer_units { get; set; }
            public string Geozone { get; set; }
            public int Geozone_index { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string PostalCode { get; set; }
            public int DigitalInputMask { get; set; }
            public string DigitalInputMask_hex { get; set; }
            public string DriverID { get; set; }
            public string DriverMessage { get; set; }
            public int EngineRPM { get; set; }
            public double EngineHours { get; set; }
            public double VehicleBatteryVolts { get; set; }
            public double EngineCoolantLevel { get; set; }
            public string EngineCoolantLevel_units { get; set; }
            public double EngineCoolantTemperature { get; set; }
            public string EngineCoolantTemperature_units { get; set; }
            public double EngineFuelUsed { get; set; }
            public string EngineFuelUsed_units { get; set; }
            public int Index { get; set; }
        }

        public class DeviceList
        {
            public string Device { get; set; }
            public string Device_desc { get; set; }
            public IList<EventData> EventData { get; set; }
        }

        public class ObjectResult
        {
            public string Account { get; set; }
            public string Account_desc { get; set; }
            public string TimeZone { get; set; }
            public IList<DeviceList> DeviceList { get; set; }
        }
    }

    
}