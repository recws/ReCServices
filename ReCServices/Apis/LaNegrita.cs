using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Microsoft.AspNet.SignalR;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Globalization;

namespace ReCServices.Apis
{
    public class LaNegrita
    {
        private readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public LaNegrita()
        {

        }

        public async void WebServiceInsertaLANEGRITA()
        {
            Token token = new Token();
            Vehicles vehicles = new Vehicles();
            Vehicle vehicle = new Vehicle();
            Device device = new Device();
            List<Evento> posicioninsertar = new List<Evento>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.ExpectContinue = false;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    Login login = new Login();
                    login.username = "lanegrita.webservice@gmail.com";
                    login.password = "wslanegrita123";

                    HttpResponseMessage responselogin = await client.PostAsync("https://gps.gc911.net/api/login", new StringContent(JsonConvert.SerializeObject(login), System.Text.Encoding.UTF8, "application/json"));
                    if (responselogin.StatusCode == HttpStatusCode.OK)
                    {
                        using (HttpContent content = responselogin.Content)
                        {
                            string data = await content.ReadAsStringAsync().ConfigureAwait(false);
                            token = JsonConvert.DeserializeObject<Token>(data);

                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.auth);
                        }
                    }
                    else
                    {
                        log.Error("Error Inserta WebService LA NEGRITA: No se pudo autenticar.");
                        return;
                    }


                    HttpResponseMessage responsegroups = await client.GetAsync("https://gps.gc911.net/api/groups/238");
                    if (responsegroups.StatusCode == HttpStatusCode.OK)
                    {
                        using (HttpContent content = responsegroups.Content)
                        {
                            string data = await content.ReadAsStringAsync().ConfigureAwait(false);
                            vehicles = JsonConvert.DeserializeObject<Vehicles>(data);
                        }
                    }
                    else
                    {
                        log.Error("Error Inserta WebService LA NEGRITA: al consultar el listado de vehiculos.");
                        return;
                    }

                    if (vehicles.vehicles == null) { return; };

                    for (int i = 0; i < vehicles.vehicles.Count; i++)
                    {
                        try
                        {
                            HttpResponseMessage responsevehicles = await client.GetAsync("https://gps.gc911.net/api/vehicles/" + vehicles.vehicles[i].ToString());
                            if (responsevehicles.StatusCode == HttpStatusCode.OK)
                            {
                                using (HttpContent content = responsevehicles.Content)
                                {
                                    string data = await content.ReadAsStringAsync().ConfigureAwait(false);
                                    vehicle = JsonConvert.DeserializeObject<Vehicle>(data);

                                    if (vehicle.info.license_plate == null)
                                    {
                                        log.Error("Error Inserta WebService LA NEGRITA: El vehiculo " + vehicle.device + " no definidas placas validas.");
                                        continue;
                                    }

                                    HttpResponseMessage responsedevices = await client.GetAsync("https://gps.gc911.net/api/devices/" + vehicle.device.ToString() + "?select=latest");
                                    if (responsedevices.StatusCode == HttpStatusCode.OK)
                                    {
                                        using (HttpContent contentdevices = responsedevices.Content)
                                        {
                                            string datadevices = await contentdevices.ReadAsStringAsync().ConfigureAwait(false);
                                            device = JsonConvert.DeserializeObject<Device>(datadevices);
                                            Evento evento = new Evento();
                                            evento.posicionGeografica.claveRastreo = vehicle.info.license_plate.ToString();
                                            evento.posicionGeografica.latitud = device.latest.loc.lat;
                                            evento.posicionGeografica.longitud = device.latest.loc.lon;
                                            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(device.latest.loc.evtime);
                                            evento.posicionGeografica.fechaAccion = dt.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss");
                                            posicioninsertar.Add(evento);
                                            //continue;
                                        }
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                        catch (Exception Ex)
                        {
                            log.Error("Error Inserta WebService LA NEGRITA: al procesar el vehiculo " + " Mensaje: " + Ex.Message);
                        }
                    }

                };


                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.ExpectContinue = false;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //var param = new { posicionGeografica = { claveRastreo = "123456", latitud = 19.847278, longitud = -99.259711, fechaAccion = DateTime.ParseExact("2017/11/08 13:12:00", "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture) } };

                    for (int i = 0; i < posicioninsertar.Count; i++)
                    {
                        try
                        {
                            HttpResponseMessage response = await client.PostAsync("https://ubi.mx/pex-logistica/mobile-pex/rutaTrabajo/guardarPosicionGPS", new StringContent(JsonConvert.SerializeObject(posicioninsertar[i]), System.Text.Encoding.UTF8, "application/json"));
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                using (HttpContent content = response.Content)
                                {
                                    string data = await content.ReadAsStringAsync().ConfigureAwait(false);
                                    var pexrespuesta = JsonConvert.DeserializeObject<PEXRespuesta>(data);
                                    if(pexrespuesta.pexResponse.codigoError == "0")
                                    {
                                        //Proceso correcto
                                    }
                                    else
                                    {
                                        log.Error("Error Inserta WebService LA NEGRITA: al insertar en el cliente. " + " Mensaje: " + pexrespuesta.pexResponse.mensajeUsuario);
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                        catch (Exception Ex)
                        {
                            log.Error("Error Inserta WebService LA NEGRITA: al insertar en el cliente.  " + (new StringContent(JsonConvert.SerializeObject(posicioninsertar), System.Text.Encoding.UTF8, "application/json")).ToString() + " Mensaje: " + Ex.Message);
                        }
                    }


                };
            }
            catch (Exception Ex)
            {
                log.Error("Error Inserta WebService LA NEGRITA: al insertar datos. " + " Mensaje: " + Ex.Message);
            }
        }

        class Login
        {
            public string username { get; set; }  //Placa
            public string password { get; set; }
        }

        /// <summary>
        /// Clase para deserializar JSON TOKEN
        /// </summary>
        private class Token
        {
            public string message { get; set; }
            public object app { get; set; }
            public string auth { get; set; }
        }

        /// <summary>
        /// Clase para deserializar JSON VEHICLES
        /// </summary>
        private class Vehicles
        {
            //public object city { get; set; }
            //public double __updated { get; set; }
            //public List<int> users { get; set; }
            //public object country { get; set; }
            public List<int> vehicles { get; set; }
            //public object contact_email { get; set; }
            //public object company_name { get; set; }
            //public object address_2 { get; set; }
            //public bool logo { get; set; }
            //public object address_1 { get; set; }
            //public object contact_name { get; set; }
            //public List<object> assets { get; set; }
            //public int id { get; set; }
            public string name { get; set; }
        }

        /// <summary>
        /// Clase para deserializar JSON VEHICLE
        /// </summary>
        private class Info
        {
            //public object range_unit { get; set; }
            //public object description { get; set; }
            //public object tank_volume { get; set; }
            //public object color { get; set; }
            //public object make { get; set; }
            //public object vin { get; set; }
            public object license_plate { get; set; }
            //public object alias { get; set; }
            //public object range { get; set; }
            //public object year { get; set; }
            //public object model { get; set; }
            //public object tank_unit { get; set; }
        }

        private class Association
        {
            public int vehicle_id { get; set; }
            public long device_id { get; set; }
            public int id { get; set; }
            public bool association { get; set; }
            public double time { get; set; }
        }

        //private class Images
        //{
        //    public bool photo { get; set; }
        //    public bool on_icon { get; set; }
        //    public bool idle_icon { get; set; }
        //    public bool icon { get; set; }
        //    public bool off_icon { get; set; }
        //}

        private class Vehicle
        {
            public Info info { get; set; }
            public List<Association> associations { get; set; }
            //public double __updated { get; set; }
            //public string name { get; set; }
            //public List<object> trackers { get; set; }
            //public object primary { get; set; }
            public long device { get; set; }
            //public object token { get; set; }
            //public List<int> groups { get; set; }
            //public Images images { get; set; }
            //public string configuration { get; set; }
            //public string type { get; set; }
            public int id { get; set; }
        }

        /// <summary>
        /// Clase para deserializar JSON DEVICE
        /// </summary>
        private class Device
        {
            public long imei { get; set; }
            public Latest latest { get; set; }
        }

        private class Latest
        {
            public Loc loc { get; set; }
            //public string prefix { get; set; }
            //public Vcounters vcounters { get; set; }
            //public Lastrx lastrx { get; set; }
            //public Data data { get; set; }
            //public Ios ios { get; set; }
            //public Counters counters { get; set; }
        }
        private class Loc
        {
            public int head { get; set; }
            public int code { get; set; }
            public long evid { get; set; }
            public int age { get; set; }
            public double lon { get; set; }
            public int mph { get; set; }
            public string label { get; set; }
            public double evtime { get; set; }
            public int source { get; set; }
            public bool valid { get; set; }
            public object moving { get; set; }
            public string evlabel { get; set; }
            public double systime { get; set; }
            public double lat { get; set; }
            //public long trip_id { get; set; }
        }
        /////////////////////////////////////////////////////




        class posicionGeografica
        {
            public string claveRastreo { get; set; }  //Placa
            public double latitud { get; set; }
            public double longitud { get; set; }
            public string fechaAccion { get; set; }
        }
        class Evento
        {
            public Evento()
            {
                posicionGeografica = new posicionGeografica();
                posicionGeografica.claveRastreo = "";
                posicionGeografica.latitud = 0;
                posicionGeografica.longitud = 0;
                posicionGeografica.fechaAccion = "";
            }
            public posicionGeografica posicionGeografica { get; set; }
        }


        private class PexResponse
        {
            public string respuesta { get; set; }
            public string codigoError { get; set; }
            public string mensajeUsuario { get; set; }
        }

        private class PEXRespuesta
        {
            public PexResponse pexResponse { get; set; }
        }
    }
}