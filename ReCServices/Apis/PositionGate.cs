using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace ReCServices.Apis
{
    public class PositionGate
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static async void PositionGate_ObtenerPosicion(string UsuarioReC, string Usuario, string Password)
        {
            DataTable DT_Data = new DataTable();
            var imei = "";
            try
            {
                DT_Data = GetData_ListaGPSxProveedor("AFN");

                if (DT_Data.Rows.Count == 0)
                {
                    return;
                }
                for (int i = 0; i < DT_Data.Rows.Count; i++)
                {
                    imei = DT_Data.Rows[i]["IMEI"].ToString();
                    var responseJson = "";

                    try
                    {
                        using (var client = new HttpClient())
                        {
                            //setup client
                            client.BaseAddress = new Uri("http://tracking.positiongate.com");
                            client.DefaultRequestHeaders.Accept.Clear();

                            //send request
                            HttpResponseMessage responseMessage = await client.GetAsync("/api/api.php?api=user&ver=1.0&key=" + Password + "&cmd=OBJECT_GET_LOCATIONS," + imei);

                            //get access token from response body
                            responseJson = await responseMessage.Content.ReadAsStringAsync();

                            responseJson = responseJson.Replace(imei, "GpsIMEI");
                            var result = JsonConvert.DeserializeObject<RootObject>(responseJson);
                            //var result = JsonConvert.DeserializeObject<List<RootObject>>(responseJson);

                            //shift + tab

                            try
                            {
                                var res = result.gpsIMEI;

                                //Consulta si ya existe la posicion, por si es repetida y no ha actualizado el equipo
                                //var imei = "";
                                string codigoevento = "";

                                string lat = res.lat.ToString();
                                string lng = res.lng.ToString();
                                //string evento = result[i].status.ToString();
                                string odometro = res.@params.odo.ToString().Split('.')[0];
                                ////string placas = ((dynamic)res[i]).Plates;
                                string velocidad = res.speed.ToString();
                                string bateria = "100";
                                string direccion = res.angle.ToString().Split('.')[0];

                                var fechahoragps = DateTime.ParseExact(res.dt_tracker, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                                fechahoragps = fechahoragps.ToUniversalTime();
                                //ver track jack
                                var fechahoraserver = DateTime.ParseExact(res.dt_server, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
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

                                WS_CONTEXT db = new WS_CONTEXT();

                                WS_GPS_InsertaSimple = db.WS_GPS_InsertaSimple(UsuarioReC, imei, codigoevento, LAT, LNG, "", true, VELOCIDAD, DIRECCION, BATERIA, ODOMETRO, fechahoragps, fechahoraserver).ToList();
                                if (WS_GPS_InsertaSimple[0].Indicador == 1)
                                {

                                }
                                else
                                {
                                    log.Error("Error al Insertar evento de " + UsuarioReC + ". IMEI: " + imei + "  " + WS_GPS_InsertaSimple[0].Mensaje + ". ");
                                }
                            }
                            catch (Exception Ex)
                            {
                                log.Error("Error PositionGate_ObtenerPosicion: " + UsuarioReC + ". " + imei + ". " + Ex.Message);
                            }
                        }
                    }
                    catch (Exception Ex)
                    {

                        log.Error("Error PositionGate_ObtenerPosicion: " + UsuarioReC + ". " + Ex.Message);

                    }
                }
            }
            catch (Exception Ex)
            {
                
                log.Error("Error PositionGate_ObtenerPosicion: " + UsuarioReC + ". " + Ex.Message);
                
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
    }

    public class Params
    {
        public string gpslev { get; set; }
        public string gsmlev { get; set; }
        public string odo { get; set; }
        public string hdop { get; set; }
        public string mcc { get; set; }
        public string mnc { get; set; }
        public string lac { get; set; }
        public string cellid { get; set; }
        public string di1 { get; set; }
        public string di2 { get; set; }
        public string di3 { get; set; }
        public string di4 { get; set; }
        public string di5 { get; set; }
        public string di6 { get; set; }
        public string di7 { get; set; }
        public string di8 { get; set; }
        public string ai1 { get; set; }
        public string ai4 { get; set; }
        public string ai5 { get; set; }
        public string evt { get; set; }
    }

    public class GpsIMEI
    {
        public string dt_server { get; set; }
        public string dt_tracker { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string altitude { get; set; }
        public string angle { get; set; }
        public string speed { get; set; }
        public Params @params { get; set; }
        public string loc_valid { get; set; }
    }

    public class RootObject
    {
        public GpsIMEI gpsIMEI { get; set; }
    }

}