using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ReCServices.Apis
{
    public static class Unicomm
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static async void Unicomm_ObtenerPosicion(string UsuarioReC,string Token, string Usuario, string Password)
        {
            try
            {
                var responseJson = "";
               
                using (var client = new HttpClient())
                {
                    
                    client.DefaultRequestHeaders.Accept.Clear();
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //var byteArray = Encoding.ASCII.GetBytes(Token);
                    var header = new AuthenticationHeaderValue("Token", Token);
                    client.DefaultRequestHeaders.Authorization = header;


                    HttpResponseMessage responseMessage = await client.GetAsync("http://ws.unicommplus.mx/consumo.json?time=30");

                    //Para consultar los economicos con imei hay que pegar esto en el navegador:
                    //http://ws.unicommplus.mx/consumo.json?time=30
                    //Cuenta: coopergdl
                    //Contraseña: c0opergdl

                    responseJson = await responseMessage.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<Eventos>(responseJson);

                    //return;



                    for (int i = 0; i < result.vehicle_states.Count; i++)
                    {
                        try
                        {
                            string imei = result.vehicle_states[i].Vehicle_ID.ToString();
                            string codigoevento = result.vehicle_states[i].Event.ToString();
                            string lat = result.vehicle_states[i].Latitude.ToString();
                            string lng = result.vehicle_states[i].Longitude.ToString();
                            string odometro = result.vehicle_states[i].Odometer.ToString().Split('.')[0];
                            string velocidad = result.vehicle_states[i].Speed == null ? "0" : result.vehicle_states[i].Speed.ToString().Split('.')[0];
                            string bateria = result.vehicle_states[i].Aux_Battery == null ? "0" : result.vehicle_states[i].Aux_Battery.ToString();
                            string direccion = result.vehicle_states[i].Course == null ? "0" : result.vehicle_states[i].Course.ToString();

                            DateTime fechahoragps = DateTime.Parse(result.vehicle_states[i].PC_Date + " " + result.vehicle_states[i].PC_Time).ToUniversalTime();     //.ToUniversalTime();

                            ////Validaciones

                            ////Conversiones de datos
                            var LAT = decimal.Parse(lat);
                            var LNG = decimal.Parse(lng);
                            var ODOMETRO = int.Parse(odometro);
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
                                log.Error("Error al Insertar evento de " + UsuarioReC + ". " + WS_GPS_InsertaSimple[0].Mensaje + ". " + imei);
                            }
                        }
                        catch (Exception Ex)
                        {
                            log.Error("Error Unicomm_ObtenerPosicion: " + UsuarioReC + ". " + responseJson + ". " + Ex.Message);
                        }
                    }


                    //for (int i = 0; i < result.Items.Count; i++)
                    //{
                    //}
                }

            }
            catch (Exception Ex)
            {
                log.Error("Error Unicomm_ObtenerPosicion: " + UsuarioReC + ". " + Ex.Message);
            }
        }


        public class VehicleState
        {
            public string Vehicle_ID { get; set; }
            public string Alias { get; set; }
            public int? Day { get; set; }
            public int? Month { get; set; }
            public int? Year { get; set; }
            public int Date_Time { get; set; }
            public double GPSTime { get; set; }
            public string PC_Date { get; set; }
            public string PC_Time { get; set; }
            //public string Street { get; set; }
            //public string City { get; set; }
            //public string State { get; set; }
            //public object Zip_Code { get; set; }
            public double? Latitude { get; set; }
            public double? Longitude { get; set; }
            public double? Speed { get; set; }
            public object Course { get; set; }
            //public double Altitude { get; set; }
            public int? Event { get; set; }
            //public object Advisory_Event { get; set; }
            //public double Distance { get; set; }
           // public int Satellites { get; set; }
            public string GPS { get; set; }
            //public int State_Code { get; set; }
            //public int Inputs { get; set; }
            //public object Advisory_Inputs { get; set; }
            //public int? Outputs { get; set; }
            //public object Message { get; set; }
            //public object Advisories { get; set; }
            //public object Nearest_Point { get; set; }
            //public int? Analog1 { get; set; }
            //public int? Analog2 { get; set; }
            //public object Analog3 { get; set; }
            //public object Analog4 { get; set; }
            //public double? HDOP { get; set; }
            //public object VDOP { get; set; }
            //public object GDOP { get; set; }
            //public object PDOP { get; set; }
            //public object Rate_of_Climb { get; set; }
            //public int? Custom1 { get; set; }
            //public object Custom2 { get; set; }
            //public object Custom3 { get; set; }
            //public object Custom4 { get; set; }
            //public object Custom5 { get; set; }
            //public object Custom6 { get; set; }
            //public object CustomText1 { get; set; }
            //public object CustomText2 { get; set; }
            //public object Nearby_Streets { get; set; }
            //public int Available_Inputs { get; set; }
            //public int? Available_Outputs { get; set; }
            //public object Driver_ID { get; set; }
            //public object VIN { get; set; }
            //public object Error_Code { get; set; }
            //public object XML { get; set; }
            //public object CustomText3 { get; set; }
            //public object Extended_Inputs { get; set; }
            //public object Available_Extended_Inputs { get; set; }
            public double? Odometer { get; set; }
            //public double? Fuel_Level { get; set; }
            public double? Battery { get; set; }
            //public object Oil_Level { get; set; }
            //public object Oil_Temperature { get; set; }
            //public object Oil_Pressure { get; set; }
            //public object Coolant_Level { get; set; }
            //public object Coolant_Temperature { get; set; }
            //public object Fuel_Economy { get; set; }
            //public object Average_Fuel_Economy { get; set; }
            public object Vehicle_Speed { get; set; }
            //public double? Engine_RPM { get; set; }
            //public double? Throttle_Position { get; set; }
            //public double? Sensor1 { get; set; }
            //public double? Sensor2 { get; set; }
            //public double? Sensor3 { get; set; }
            //public double? Sensor4 { get; set; }
            //public object Sensor5 { get; set; }
            //public object Sensor6 { get; set; }
            //public object Sensor7 { get; set; }
            //public object Sensor8 { get; set; }
            public int? Aux_Battery { get; set; }
            //public object Engine_ID { get; set; }
            //public int Update_Number { get; set; }
        }

        public class Eventos
        {
            public IList<VehicleState> vehicle_states { get; set; }
        }
    }
}