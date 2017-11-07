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
    public class Troncalnet
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static async void Troncalnet_ObtenerPosicion(string UsuarioReC, string Usuario, string Password)
        {
            try { 
            var responseJson = "";
                var responseJson2 = "";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://us.mzoneweb.net");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var byteArray = Encoding.ASCII.GetBytes("coopergdl:c0opergdl");
                    var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    client.DefaultRequestHeaders.Authorization = header;

                    var fini = DateTime.Now.AddHours(-5).ToString("yyyyMMddTHHmmss");
                    var ffin = DateTime.Now.ToString("yyyyMMddTHHmmss");

                    HttpResponseMessage responseMessage = await client.GetAsync("/api/v2/vehiclegroups/93121805-644a-4738-85d8-392906a3fc4e/events/" + fini + "/" + ffin + ".json");

                    //Para consultar los economicos con imei hay que pegar esto en el navegador:
                    //https://us.mzoneweb.net/api/v2/vehiclegroups/93121805-644a-4738-85d8-392906a3fc4e/units.json
                    //Cuenta: coopergdl
                    //Contraseña: c0opergdl

                    responseJson = await responseMessage.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<Eventos>(responseJson);

                    /////////////////////////////////////////////////////////////////////////////////////////
                    using (var client2 = new HttpClient())
                    {
                        client2.BaseAddress = new Uri("https://us.mzoneweb.net");
                        client2.DefaultRequestHeaders.Accept.Clear();
                        client2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var byteArray2 = Encoding.ASCII.GetBytes("coopergdl:c0opergdl");
                        var header2 = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray2));
                        client2.DefaultRequestHeaders.Authorization = header2;


                        HttpResponseMessage responseMessage2 = await client2.GetAsync("/api/v2/vehiclegroups/93121805-644a-4738-85d8-392906a3fc4e/lastknownpositions.json");

                        //Para consultar los economicos con imei hay que pegar esto en el navegador:
                        //https://us.mzoneweb.net/api/v2/vehiclegroups/93121805-644a-4738-85d8-392906a3fc4e/lastknownpositions.json
                        //Cuenta: coopergdl
                        //Contraseña: c0opergdl

                        responseJson2 = await responseMessage2.Content.ReadAsStringAsync();

                        var result2 = JsonConvert.DeserializeObject<Eventos>(responseJson2);

                        //////// busca por unitid y fechahora para obtener velocidad, odometro y heading
                        for (int i = 0; i < result2.Items.Count; i++) //Result2 es el array con elementos unicos
                        {
                            for (int j = 0; j < result.Items.Count; j++)
                            {
                                if (result.Items[j].UnitId == result2.Items[i].UnitId && result.Items[j].LocalTimestamp == result2.Items[i].LocalTimestamp)
                                {
                                    result2.Items[i].Speed = result.Items[j].Speed;
                                    result2.Items[i].Odometer = result.Items[j].Odometer;                                    
                                    result2.Items[i].Direction = result.Items[j].Direction;
                                    result2.Items[i].EventTypeId = result.Items[j].EventTypeId;
                                    result2.Items[i].EventTypeDescription = result.Items[j].EventTypeDescription;
                                    break;                            
                                }
                            }
                        }


                        for (int i = 0; i < result2.Items.Count; i++)
                        {
                            try
                            {
                                string imei = result2.Items[i].UnitId.ToString();
                                string codigoevento = result2.Items[i].EventTypeId.ToString();
                                string lat = result2.Items[i].Position[1].ToString();
                                string lng = result2.Items[i].Position[0].ToString();
                                string odometro = result2.Items[i].Odometer.ToString().Split('.')[0];
                                string velocidad = result2.Items[i].Speed.ToString().Split('.')[0];
                                string bateria = "100";
                                string direccion = result2.Items[i].Direction.ToString().Split('.')[0];

                                DateTime fechahoragps = result2.Items[i].LocalTimestamp.ToUniversalTime();

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

                                WS_CONTEXT db = new WS_CONTEXT();

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
                                log.Error("Error Troncalnet_ObtenerPosicion: " + UsuarioReC + ". " + responseJson + ". " + Ex.Message);
                            }
                        }
                    }

                    //for (int i = 0; i < result.Items.Count; i++)
                    //{
                    //}
                }
                
            }
            catch (Exception Ex)
            {
                log.Error("Error Troncalnet_ObtenerPosicion: " + UsuarioReC + ". "  + Ex.Message);
            }
        }
            


        public class Item
        {
            public object Id { get; set; }
            public string UnitId { get; set; }
            public double Direction { get; set; }
            public int EventTypeId { get; set; }
            public string EventTypeDescription { get; set; }
            public DateTime LocalTimestamp { get; set; }
            public int Odometer { get; set; }
            public IList<double> Position { get; set; }
            public int RPM { get; set; }
            public double Speed { get; set; }
            public string UnitOfDistanceCode { get; set; }
        }

        public class Eventos
        {
            public int Page { get; set; }
            public int PageSize { get; set; }
            public int TotalResults { get; set; }
            public IList<Item> Items { get; set; }
            public bool HasMoreResults { get; set; }
        }
    }
}