using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace ReCServices.Apis
{
    public static class Wialon
    {

        //El token se obtiene en la siguiente url con el usuario y contraseña. Este token no caduca.
        //https://hosting.wialon.com/login.html?client_id=wialon&access_type=-1&activation_time=0&duration=2592000&flags=6&response_type=token&svc_error=1011
        //La documentacion se encuentra en el siguiente url
        //https://sdk.wialon.com/wiki/en/kit/remoteapi/apiref/core/search_items
        //La plataforma se accesa en el siguiente url
        //https://hosting.wialon.com/?token=


        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static string EID { get; set; }
        public static string TOKEN { get; set; }

        public static async void WIALON_ObtenerToken(string UsuarioReC,string Token)
        {
            
            var responseJson = "";
            try
            {
                using (var client = new HttpClient())
                {
                    //setup client
                    client.BaseAddress = new Uri("https://hst-api.wialon.com");
                    client.DefaultRequestHeaders.Accept.Clear();

                    //setup login data
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                    //new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("svc", "token/login"),
                    new KeyValuePair<string, string>("params", "{ \"token\":\"" + TOKEN + "\"}")
                });

                    //send request
                    HttpResponseMessage responseMessage = await client.PostAsync("/wialon/ajax.html", formContent);
                    //HttpResponseMessage responseMessage = await client.PostAsync("/web%20services/ws_last_position/ws_last_position.asmx/GetLastPosition_02?User=" + Usuario  + "&Password=" + Password + "&Page=", formContent);

                    //get access token from response body
                    responseJson = await responseMessage.Content.ReadAsStringAsync();

                    if (responseJson == "{\"error\":8, \"reason\":\"INVALID_AUTH_TOKEN\"}\n")
                    {
                        log.Error("Error WIALON_ObtenerPosicion: " + UsuarioReC + ". " + responseJson + ". " );
                    }

                    var result = JsonConvert.DeserializeObject<LoginObject>(responseJson);

                    EID = result.eid;
                    log.Info("WebService WIALON Autenticado: " + UsuarioReC + ". ");
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
                    log.Error("Error WIALON_ObtenerPosicion: " + UsuarioReC + ". " + responseJson + ". " + Ex.Message);
                }
                EID = "";
            }
        }
        public static async void WIALON_ObtenerPosicion(string UsuarioReC, string Usuario, string Password, string Token)
        {
            TOKEN = Token;
            var responseJson = "";
            try
            {
                if (TOKEN == null || TOKEN == "")
                {
                    WIALON_ObtenerToken(UsuarioReC, TOKEN);
                    return;
                }

                using (var client = new HttpClient())
                {
                    //setup client
                    client.BaseAddress = new Uri("https://hst-api.wialon.com");
                    client.DefaultRequestHeaders.Accept.Clear();
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //setup login data
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("svc", "core/search_items"),
                    new KeyValuePair<string, string>("params", "{ \"spec\":{ \"itemsType\":\"avl_unit\",\"propName\":\"sys_name\",\"propValueMask\":\"*\",\"sortType\":\"sys_name\"},\"force\":1,\"flags\":1025,\"from\":0,\"to\":0}"),                    
                    new KeyValuePair<string, string>("sid", EID)
                    });

                    //send request
                    HttpResponseMessage responseMessage = await client.PostAsync("/wialon/ajax.html", formContent);
                    //HttpResponseMessage responseMessage = await client.PostAsync("/web%20services/ws_last_position/ws_last_position.asmx/GetLastPosition_02?User=" + Usuario  + "&Password=" + Password + "&Page=", formContent);

                    //get access token from response body

                    //orgininal
                    responseJson = await responseMessage.Content.ReadAsStringAsync();
                    //responseMessage.Content.
                    
                    if (responseJson.Contains("error"))
                    {
                        //TOKEN = "";
                        WIALON_ObtenerToken(UsuarioReC, TOKEN);
                        return;
                    }

                    var result = JsonConvert.DeserializeObject<SearchResult>(responseJson);

                    for ( int i = 0; i < result.items.Count; i++)
                    {
                        //Consulta si ya existe la posicion, por si es repetida y no ha actualizado el equipo
                        string imei = result.items[i].id.ToString();
                        string codigoevento = result.items[i].lmsg == null ? "" : result.items[i].lmsg.tp.ToString();

                        if(result.items[i].pos == null)
                        {
                            continue;
                        }

                        string lat = result.items[i].pos.y.ToString(); 
                        string lng = result.items[i].pos.x.ToString();
                        //string evento = result.items[i].status.ToString();
                        string odometro = result.items[i].lmsg.p.mileage.ToString();
                        ////string placas = ((dynamic)res[i]).Plates;
                        string velocidad = result.items[i].pos.s.ToString();
                        string bateria = result.items[i].lmsg.p.battery.ToString();
                        string direccion = result.items[i].pos.c.ToString();

                        DateTime fechahoragps = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(result.items[i].pos.t);

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
                    log.Error("Error WIALON_ObtenerPosicion: " + UsuarioReC + ". " + responseJson + ". " + Ex.Message);
                }
            }
        }
    }


    public class LoginObject
    {
        public string host { get; set; }
        public string eid { get; set; }
        public string au { get; set; }
        public int tm { get; set; }
        public string wsdk_version { get; set; }
        public string base_url { get; set; }
        //public User user { get; set; }
        public string token { get; set; }
        public string th { get; set; }
        //public Classes classes { get; set; }
        //public Features features { get; set; }
    }

    public class SearchSpec
    {
        public string itemsType { get; set; }
        public string propName { get; set; }
        public string propValueMask { get; set; }
        public string sortType { get; set; }
        public string propType { get; set; }
        public string or_logic { get; set; }
    }

    public class Pos
    {
        public int t { get; set; }
        public int f { get; set; }
        //public int lc { get; set; }
        public double y { get; set; }
        public double x { get; set; }
        public double z { get; set; }
        public int s { get; set; }
        public int c { get; set; }
        public int sc { get; set; }
    }

    public class P
    {
        public int gps_acc { get; set; }
        public int mcc { get; set; }
        public int mnc { get; set; }
        public int lac { get; set; }
        public int cell_id { get; set; }
        public int number { get; set; }
        public int mileage { get; set; }
        public int battery { get; set; }
        public int state { get; set; }
        public int ign { get; set; }
        public int buffer { get; set; }
        public string report_type { get; set; }
        public int? param239 { get; set; }
        public double? pwr_ext { get; set; }
        public int? battery_charge { get; set; }
    }

    public class Lmsg
    {
        public int t { get; set; }
        public int f { get; set; }
        public string tp { get; set; }
        //public Pos pos { get; set; }
        //public int i { get; set; }
        //public int o { get; set; }
        //public int lc { get; set; }
        public P p { get; set; }
    }

    public class Item
    {
        public string nm { get; set; }
        //public int cls { get; set; }
        public int id { get; set; }
        public int mu { get; set; }
        public Pos pos { get; set; }
        public Lmsg lmsg { get; set; }
        //public object uacl { get; set; }
    }

    public class SearchResult
    {
        //public SearchSpec searchSpec { get; set; }
        public int dataFlags { get; set; }
        public int totalItemsCount { get; set; }
        public int indexFrom { get; set; }
        public int indexTo { get; set; }
        public IList<Item> items { get; set; }
    }

}
