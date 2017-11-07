using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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


namespace ReCServices
{
    public class APIS
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static async void KOSMOS_ObtenerPosicion(string UsuarioReC, string Usuario, string Password) {
            var responseJson = "";
            try
            {                
                using (var client = new HttpClient())
                {
                    //setup client
                    client.BaseAddress = new Uri("http://www.utrax2.com/");
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

                    //load into XElement
                    XElement doc = XElement.Parse(responseJson);
                    var res = doc.ToDynamicList();

                    for (int i = 0; i < res.Count; i++)
                    {
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
                        if (velocidad.IndexOf(".") >=0)
                        {
                            velocidad = velocidad.Remove(velocidad.IndexOf("."));
                        }
                        direccion = direccion == "~" ? "0" : direccion;
                        if(direccion.IndexOf(".")>=0)
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

                        WS_CONTEXT db = new WS_CONTEXT();

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
            catch(Exception Ex)
            {
                if (Ex.Message == "'System.Dynamic.ExpandoObject' no contiene una definición para 'Latitude'.")
                {
                    //No guarda nada en el log por que aveces no viene completa la trama
                }
                else
                {
                    log.Error("Error KOSMOS_ObtenerPosicion: " + UsuarioReC + ". " + responseJson + ". " + Ex.Message);
                }
            }
        }

        public static async void REESER_ObtenerPosicion(string UsuarioReC, string Usuario, string Password)
        {
            var responseJson = "";
            try
            {
                using (var client = new HttpClient())
                {
                    //setup client
                    client.BaseAddress = new Uri("http://rastreo.resser.com/api/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var byteArray = Encoding.ASCII.GetBytes(Usuario + ":" + Password);
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));


                    //var credentials = new System.Net.NetworkCredential(Usuario, Password);
                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
                    //setup login data
                    var offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                        //new KeyValuePair<string, string>("grant_type", "password"),
                        new KeyValuePair<string, string>("Id", "11601"),
                        new KeyValuePair<string, string>("TimeZone", offset.ToString()),
                        //new KeyValuePair<string, string>("Page", ""),
                    });

                    //send request
                    HttpResponseMessage responseMessage = await client.GetAsync("lastpositionreport?id=11601&timezone=" + offset.Hours.ToString());
                    
                    responseJson = await responseMessage.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<RootObject>(responseJson);

                    for (int i = 0; i < result.items.Count; i++)
                    {
                        //Consulta si ya existe la posicion, por si es repetida y no ha actualizado el equipo
                        string imei = result.items[i].id.ToString();
                        string codigoevento = result.items[i].status.ToString();

                        string lat = result.items[i].position.latitude.ToString();
                        string lng = result.items[i].position.longitude.ToString();
                        string evento = result.items[i].status.ToString();
                        string odometro = result.items[i].odometer.ToString();
                        //string placas = ((dynamic)res[i]).Plates;
                        string velocidad = result.items[i].position.speed.ToString();
                        string bateria = "";
                        try
                        {
                            bateria = result.items[i].batteryPercentage == null ? "0" : result.items[i].batteryPercentage.ToString();
                        }
                        catch (Exception Ex)
                        {
                            bateria = "";
                        }

                        int index = result.items[i].position.orientation.ToString().IndexOf("°");
                        string direccion = (index > 0 ? result.items[i].position.orientation.ToString().Substring(0, index) : "0");
                        string fechahoragps = result.items[i].position.gps_date + " " + result.items[i].position.gps_time;
                        string fechahoraservidor = result.items[i].date + " " + result.items[i].time;

                        //Validaciones
                       
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
                        var dateTimegps = DateTime.ParseExact(fechahoragps, "M/d/yyyy h:m tt", CultureInfo.InvariantCulture);
                        dateTimegps = dateTimegps.ToUniversalTime();
                        var dateTimeservidor = DateTime.ParseExact(fechahoraservidor, "M/d/yyyy h:m:ss tt", CultureInfo.InvariantCulture);
                        dateTimeservidor = dateTimeservidor.ToUniversalTime();

                        //Si no es repetida la inserta
                        List<WS_GPS_InsertaSimple_Result> WS_GPS_InsertaSimple;

                        WS_CONTEXT db = new WS_CONTEXT();

                        WS_GPS_InsertaSimple = db.WS_GPS_InsertaSimple(UsuarioReC, imei, codigoevento, LAT, LNG, "", true, VELOCIDAD, DIRECCION, BATERIA, ODOMETRO, dateTimegps, dateTimeservidor).ToList();
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
                    log.Error("Error REESER_ObtenerPosicion: " + UsuarioReC + ". " + responseJson + ". " + Ex.Message);
                }
            }
        }

    }

    // XElement extensions
    public static class XElementExtensions
    {
        // extended the XElement with a method called DoDynamicList
        public static List<dynamic> ToDynamicList(this XElement elements, List<dynamic> data = null)
        {
            // if we already have items in the data object, we will append to them
            // if not create a new data object
            if (data == null)
            {
                data = new List<dynamic>();
            }

            // loop through child elements
            foreach (XElement element in elements.Elements())
            {
                // define an Expando Dynamic
                dynamic item = new ExpandoObject();

                // cater for attributes as properties
                if (element.HasAttributes)
                {
                    foreach (var attribute in element.Attributes())
                    {
                        ((IDictionary<string, object>)item).Add(attribute.Name.LocalName, attribute.Value);
                    }
                }

                // cater for child nodes as properties, or child objects
                if (element.HasElements)
                {
                    foreach (XElement subElement in element.Elements())
                    {
                        // if sub element has child elements
                        if (subElement.HasElements)
                        {
                            // using a bit of recursion lets us cater for an unknown chain of child elements
                            ((IDictionary<string, object>)item).Add(subElement.Name.LocalName, subElement.ToDynamicList());
                        }
                        else
                        {
                            ((IDictionary<string, object>)item).Add(subElement.Name.LocalName, subElement.Value);
                        }
                    }
                }

                data.Add(item);
            }

            return data;
        }
    }

    public class XmlToDynamic
    {
        public static void Parse(dynamic parent, XElement node)
        {
            if (node.HasElements)
            {
                if (node.Elements(node.Elements().First().Name.LocalName).Count() > 1)
                {
                    //list
                    var item = new ExpandoObject();
                    var list = new List<dynamic>();
                    foreach (var element in node.Elements())
                    {
                        Parse(list, element);
                    }

                    AddProperty(item, node.Elements().First().Name.LocalName, list);
                    AddProperty(parent, node.Name.ToString(), item);
                }
                else
                {
                    var item = new ExpandoObject();

                    foreach (var attribute in node.Attributes())
                    {
                        AddProperty(item, attribute.Name.ToString(), attribute.Value.Trim());
                    }

                    //element
                    foreach (var element in node.Elements())
                    {
                        Parse(item, element);
                    }

                    AddProperty(parent, node.Name.ToString(), item);
                }
            }
            else
            {
                AddProperty(parent, node.Name.ToString(), node.Value.Trim());
            }
        }

        private static void AddProperty(dynamic parent, string name, object value)
        {
            if (parent is List<dynamic>)
            {
                (parent as List<dynamic>).Add(value);
            }
            else
            {
                (parent as IDictionary<String, object>)[name] = value;
            }
        }
    }



    public class Position
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string gps_date { get; set; }
        public string gps_time { get; set; }
        public int speed { get; set; }
        public string orientation { get; set; }
        public string geocode { get; set; }
    }

    public class Vehicle
    {
        public int id { get; set; }
        public string description { get; set; }
    }

    public class Item
    {
        public int id { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public Position position { get; set; }
        public Vehicle vehicle { get; set; }
        public double odometer { get; set; }
        public string status { get; set; }
        public int notification { get; set; }
        public object temperature { get; set; }
        public int? batteryPercentage { get; set; }
        //public double batteryLevel { get; set; } //Se quita por que de repente da error por el tipo de datos   Error converting value {null} to type 'System.Double'.
    }

    public class RootObject
    {
        public List<Item> items { get; set; }
        public string lastDate { get; set; }
    }
}