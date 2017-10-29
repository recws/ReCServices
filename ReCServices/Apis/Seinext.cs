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

namespace ReCServices.Apis
{
    public static class Seinext
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static async void Seinext_ObtenerPosicion(string UsuarioReC, string Usuario, string Password)
        {
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

                    var result = JsonConvert.DeserializeObject<RootObject>(json2);

                    //var result = JsonConvert.DeserializeObject<List<RootObject>>(json2);
                    //--


                    if (result.otros == null)
                    {
                        return;
                    }
                    
                    //result[0].otros.equipo[0].fechahorag
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

    }
}