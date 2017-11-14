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
using System.Xml;
using Newtonsoft.Json.Linq;

namespace ReCServices.Apis
{
    public static class Seinext
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static async void Seinext_ObtenerPosicion(string UsuarioReC, string Usuario, string Password)
        {
            //Ejemplo
            //{ "xml":{ "version":"1.0","encoding":"iso-8859-1"},"otros":{ "equipo":[{"neconomicov":"AVEN-LB-47-866","unitidv":"954018","latitudv":"19.708854","longitudv":"-099.208836","velocidadv":"0","direccionv":"000� (Norte)","gasolinav":"ND","ubicacionv":"HUEHUETOCA;M�XICO;MEXICO","poiv":"","fechahorag":"11/13/2017 20:20","fechahorav":"11/13/2017 20:19","mensajev":"AUTOREPORTE POR TIEMPO (IGN OFF)","temperaturav":"ND"},{"neconomicov":"AVEN-MYK-97-07","unitidv":"012642000528039","latitudv":"19.349863","longitudv":"-99.12103","velocidadv":"0","direccionv":"0� (Norte)","gasolinav":"0","ubicacionv":"PASEO DEL RIO;PASEOS DE TAXQUE�A;COYOACAN;DISTRITO FEDERAL;MEXICO","poiv":"","fechahorag":"11/13/2017 20:20","fechahorav":"11/13/2017 20:20","mensajev":"AUTOREPORTE POR TIEMPO","temperaturav":"ND"}]}}

            var responseJson = "";
            try
            {
                using (var client = new HttpClient())
                {
                    //setup client
                    client.BaseAddress = new Uri("http://p22gateway.dyndns.org");
                    client.DefaultRequestHeaders.Accept.Clear();

                    var formContent = new FormUrlEncodedContent(new[]
                    {
                    //new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("USUARIO", Usuario),
                    new KeyValuePair<string, string>("PASSWORD", Password)
                });

                    //send request
                    HttpResponseMessage responseMessage = await client.PostAsync("/webservices/seinext/unidades2.asp", formContent);
                    //HttpResponseMessage responseMessage = await client.PostAsync("/web%20services/ws_last_position/ws_last_position.asmx/GetLastPosition_02?User=" + Usuario  + "&Password=" + Password + "&Page=", formContent);

                    //XML Response
                    responseJson = await responseMessage.Content.ReadAsStringAsync();
                    //-----
                    System.Xml.XmlDocument doc2 = new System.Xml.XmlDocument();
                    doc2.LoadXml(responseJson);
                    string json2 = JsonConvert.SerializeXmlNode(doc2);

                    json2 = System.Text.RegularExpressions.Regex.Replace(json2, "string", "Dataset");
                    json2 = json2.Replace("?", "");
                    json2 = json2.Replace("#", "");
                    json2 = System.Text.RegularExpressions.Regex.Replace(json2, "@", "");

                    //un elemento
                    //json2 = "{\"xml\":{\"version\":\"1.0\",\"encoding\":\"iso-8859-1\"},\"otros\":{\"equipo\":{\"neconomicov\":\"AVEN-853-DF-1\",\"unitidv\":\"954016\",\"latitudv\":\"19.434205\",\"longitudv\":\"-099.103728\",\"velocidadv\":\"0\",\"direccionv\":\"000� (Norte)\",\"gasolinav\":\"ND\",\"ubicacionv\":\"DE LA INDUSTRIA AV;MOCTEZUMA 2A SECCION;VENUSTIANO CARRANZA;DISTRITO FEDERAL;MEXICO\",\"poiv\":\"\",\"fechahorag\":\"11/13/2017 19:47\",\"fechahorav\":\"11/13/2017 19:47\",\"mensajev\":\"AUTOREPORTE POR TIEMPO (IGN OFF)\",\"temperaturav\":\"ND\"}}}";

                    //dos elementos
                    //json2 = "{\"xml\":{\"version\":\"1.0\",\"encoding\":\"iso-8859-1\"},\"otros\":{\"equipo\":[{\"neconomicov\":\"AVEN-LB-47-866\",\"unitidv\":\"954018\",\"latitudv\":\"19.708975\",\"longitudv\":\"-099.208777\",\"velocidadv\":\"0\",\"direccionv\":\"000� (Norte)\",\"gasolinav\":\"ND\",\"ubicacionv\":\"HUEHUETOCA;M�XICO;MEXICO\",\"poiv\":\"\",\"fechahorag\":\"11/13/2017 19:50\",\"fechahorav\":\"11/13/2017 19:50\",\"mensajev\":\"AUTOREPORTE POR TIEMPO (IGN OFF)\",\"temperaturav\":\"ND\"},{\"neconomicov\":\"AVEN-MYK-97-07\",\"unitidv\":\"012642000528039\",\"latitudv\":\"19.349886\",\"longitudv\":\"-99.12112\",\"velocidadv\":\"0\",\"direccionv\":\"0� (Norte)\",\"gasolinav\":\"0\",\"ubicacionv\":\"PASEO DEL RIO;PASEOS DE TAXQUE�A;COYOACAN;DISTRITO FEDERAL;MEXICO\",\"poiv\":\"\",\"fechahorag\":\"11/13/2017 19:50\",\"fechahorav\":\"11/13/2017 19:50\",\"mensajev\":\"AUTOREPORTE POR TIEMPO\",\"temperaturav\":\"ND\"}]}}";

                    //sin elementos
                    //json2 = "{\"xml\":{\"version\":\"1.0\",\"encoding\":\"iso-8859-1\"},\"otros\":\"\"}";


                    try
                    {
                        var result = JsonConvert.DeserializeObject<RootObject>(json2);
                        if (result.otros == null)
                        {
                            return;
                        }
                         var res = result.otros.equipo;

                        for (int i = 0; i < res.Count; i++)
                        {

                            //Consulta si ya existe la posicion, por si es repetida y no ha actualizado el equipo
                            string imei = res[i].unitidv.ToString();
                            string codigoevento = "";

                            string lat = res[i].latitudv.ToString();
                            string lng = res[i].longitudv.ToString();

                            string odometro = "0";
                            ////string placas = ((dynamic)res[i]).Plates;
                            string velocidad = res[i].velocidadv.ToString().Split('.')[0];
                            string bateria = "0";
                            string direccion = res[i].direccionv.ToString();
                            direccion = System.Text.RegularExpressions.Regex.Replace(direccion, "[^0-9]+", "");

                            //2017 / 07 / 07 20:37:06
                            var fechahoragps = DateTime.ParseExact(res[i].fechahorag, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
                            fechahoragps = fechahoragps.ToUniversalTime();
                            var fechahoraserver = DateTime.ParseExact(res[i].fechahorav, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
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
                                log.Error("Error al Insertar evento de " + UsuarioReC + ". " + " IMEI: " + imei + ". " + WS_GPS_InsertaSimple[0].Mensaje + ". ");
                            }
                        }
                    }
                    catch
                    {
                        var result = JsonConvert.DeserializeObject<RootObject2>(json2);
                        
                        if (result.otros.equipo == null)
                        {
                            return;
                        }
                        var res = result.otros.equipo;                       

                        //Consulta si ya existe la posicion, por si es repetida y no ha actualizado el equipo
                        string imei = res.unitidv.ToString();
                        string codigoevento = "";

                        string lat = res.latitudv.ToString();
                        string lng = res.longitudv.ToString();

                        string odometro = "0";
                        ////string placas = ((dynamic)res[i]).Plates;
                        string velocidad = res.velocidadv.ToString().Split('.')[0];
                        string bateria = "0";
                        string direccion = res.direccionv.ToString();
                        direccion = System.Text.RegularExpressions.Regex.Replace(direccion, "[^0-9]+", "");

                        //2017 / 07 / 07 20:37:06
                        var fechahoragps = DateTime.ParseExact(res.fechahorag, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
                        fechahoragps = fechahoragps.ToUniversalTime();
                        var fechahoraserver = DateTime.ParseExact(res.fechahorav, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
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
                    log.Error("Error RedGPS_ObtenerPosicion: " + UsuarioReC + ". " + responseJson + ". " + Ex.Message);
                }
            }
        }

        public class Xml
        {
            public string version { get; set; }
            public string encoding { get; set; }
        }

        public class Equipo
        {
            public string neconomicov { get; set; }
            public string unitidv { get; set; }
            public string latitudv { get; set; }
            public string longitudv { get; set; }
            public string velocidadv { get; set; }
            public string direccionv { get; set; }
            public string gasolinav { get; set; }
            public string ubicacionv { get; set; }
            public string poiv { get; set; }
            public string fechahorag { get; set; }
            public string fechahorav { get; set; }
            public string mensajev { get; set; }
            public string temperaturav { get; set; }
        }

        public class Otros
        {
            public List<Equipo> equipo { get; set; }
        }

        public class RootObject
        {
            public Xml xml { get; set; }
            public Otros otros { get; set; }
        }


        //Un elemento

        //public class Xml2
        //{
        //    public string version { get; set; }
        //    public string encoding { get; set; }
        //}

        //public class Equipo2
        //{
        //    public string neconomicov { get; set; }
        //    public string unitidv { get; set; }
        //    public string latitudv { get; set; }
        //    public string longitudv { get; set; }
        //    public string velocidadv { get; set; }
        //    public string direccionv { get; set; }
        //    public string gasolinav { get; set; }
        //    public string ubicacionv { get; set; }
        //    public string poiv { get; set; }
        //    public string fechahorag { get; set; }
        //    public string fechahorav { get; set; }
        //    public string mensajev { get; set; }
        //    public string temperaturav { get; set; }
        //}

        public class Otros2
        {
            public Equipo equipo { get; set; }
        }

        public class RootObject2
        {
            public Xml xml { get; set; }
            public Otros2 otros { get; set; }
        }
    }
}