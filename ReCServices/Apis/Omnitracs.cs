using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Globalization;

namespace ReCServices.Apis
{
    public class Omnitracs
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static string Token { get; set; }

        //public static ReCServices.WS_OmnitracsMasetto_Service.OTSWebSvcsClient ws_Masetto = new ReCServices.WS_OmnitracsMasetto_Service.OTSWebSvcsClient();


        public static void Omnitracs_ObtenerPosicion(string UsuarioReC, string Usuario, string Password)
        {
            var responseJson = "";
            try
            {
                string xmlstring = "";

                //Nota para agregar otro proveedor, no olvidar agregar la referencia de servicio correspondiente y poner usuario y contraseña 
                //      en el archivo web.config 

                //if (UsuarioReC == "WS_Masetto")
                //{
                //var predequeue = ws_Masetto.dequeue2(1, 0);
                //var dequeue = ws_Masetto.dequeue2(1, predequeue.transactionIdOut);
                //ReCServices.WS_OmnitracsMasetto_Service.OTSWebSvcsClient ws_Masetto = new ReCServices.WS_OmnitracsMasetto_Service.OTSWebSvcsClient();
                ReCServices.WS_OmnitracsMasetto_Service.OTSWebSvcsClient ws_Masetto = new ReCServices.WS_OmnitracsMasetto_Service.OTSWebSvcsClient();
                //ws_Masetto.Open();
                long transactionidout = 0;
                var dequeue = ws_Masetto.dequeue2(1, 0);
                transactionidout = dequeue.transactionIdOut;
                xmlstring = System.Text.Encoding.Default.GetString(dequeue.transactions);
                ws_Masetto.Close();

                //var dequeue2 = ws_Masetto.dequeue2(1, dequeue.transactionIdOut);
                //var xmlstring2 = System.Text.Encoding.Default.GetString(dequeue2.transactions);
                //}
                for (int i = 0; i < 2; i++)
                {
                    if (i == 1)  //Si es la segunda vuelta 
                    {
                        //ReCServices.WS_OmnitracsMasetto_Service.OTSWebSvcsClient ws_Masetto2 = new ReCServices.WS_OmnitracsMasetto_Service.OTSWebSvcsClient();
                        ReCServices.WS_OmnitracsMasetto_Service.OTSWebSvcsClient ws_Masetto2 = new ReCServices.WS_OmnitracsMasetto_Service.OTSWebSvcsClient();
                        //ws_Masetto2.Open();
                        var dequeue2 = ws_Masetto2.dequeue2(1, transactionidout);
                        xmlstring = System.Text.Encoding.Default.GetString(dequeue2.transactions);
                        ws_Masetto2.Close();
                    }

                    if (xmlstring == "")
                    {
                        return;
                    }



                    System.Xml.XmlDocument doc2 = new System.Xml.XmlDocument();
                    doc2.LoadXml(xmlstring);
                    string json2 = JsonConvert.SerializeXmlNode(doc2);
                    json2 = System.Text.RegularExpressions.Regex.Replace(json2, "string", "Dataset");
                    json2 = json2.Replace("?", "");
                    json2 = json2.Replace("#", "");
                    json2 = System.Text.RegularExpressions.Regex.Replace(json2, "@", "");
                    json2 = json2.Replace("T.2.06.0", "T2060");


                    var res = new Tran();
                    try
                    {
                        //Intenta deserializar si es un array
                        var result = JsonConvert.DeserializeObject<ObjectResult>(json2);
                        res = result.tranBlock.tran;
                        InsertaIndividual(UsuarioReC, res);
                    }
                    catch (Exception Ex1)
                    {
                        try
                        {
                            //Si el try anterior da error es por que no es array e intenta deserializar simple
                            json2 = System.Text.RegularExpressions.Regex.Replace(json2, "\"tranBlock\"", "\"tranBlocks\"");
                            //json2 = json2.Replace("tranBlock", "tranBlocks");
                            var result = JsonConvert.DeserializeObject<ObjectResult>(json2);
                            //res = result.tranBlocks.tran;
                            for (int j = 0; j < result.tranBlocks.tran.Count; j++)
                            {
                                InsertaIndividual(UsuarioReC, result.tranBlocks.tran[j]);
                            }
                        }
                        catch (Exception Ex2)
                        {
                            //continue;
                        }
                    }



                    //}
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
                    log.Error("Error Omnitracs_ObtenerPosicion: " + UsuarioReC + ". " + responseJson + ". " + Ex.Message);
                }
            }
        }

        private static void InsertaIndividual(string UsuarioReC, Tran res)
        {
            try
            {

                string imei = res.T2060.equipment.unitAddress.ToString();
                string codigoevento = "";

                string lat = res.T2060.position.lat;
                string lng = res.T2060.position.lon;
                //string evento = res[i].status.ToString();
                string odometro = "0";
                ////string placas = ((dynamic)res[i]).Plates;
                string velocidad = "0";
                string bateria = "100";
                string direccion = "0";
                //2017 / 07 / 07 20:37:06
                //DateTime fechahoragps = DateTime.ParseExact(res[i].T2060.position.posTS);
                var fechahoragps = DateTime.ParseExact(res.T2060.position.posTS, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
                fechahoragps = fechahoragps.ToUniversalTime();
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
                    log.Error("Error al Insertar evento de " + UsuarioReC + ". " + " IMEI: " + imei + ". " + WS_GPS_InsertaSimple[0].Mensaje + ". ");
                }

            }
            catch (Exception Ex3)
            {
                log.Error("Error al Insertar evento de " + UsuarioReC + ". " + Ex3.Message + ". ");
            }
        }

        public class Xml
        {
            public string version { get; set; }
            public string encoding { get; set; }
        }

        public class Equipment
        {
            public string ID { get; set; }
            public string equipType { get; set; }
            public string unitAddress { get; set; }
            public string mobileType { get; set; }
        }

        public class Position
        {
            public string lon { get; set; }
            public string lat { get; set; }
            public string posTS { get; set; }
        }

        public class T2060
        {
            public string eventTS { get; set; }
            public Equipment equipment { get; set; }
            public Position position { get; set; }
            //public object proximity { get; set; }
            public string posType { get; set; }
            public string ignitionStatus { get; set; }
            public string tripStatus { get; set; }
            public string ltdDistance { get; set; }
        }

        public class Tran
        {
            public string ID { get; set; }
            public string companyID { get; set; }
            public string auxID { get; set; }
            public T2060 T2060 { get; set; }
        }

        public class TranBlock
        {
            public Omnitracs.Tran tran { get; set; }
        }
        public class TranBlocks
        {
            public List<Omnitracs.Tran> tran { get; set; }
        }

        public class ObjectResult
        {
            public Omnitracs.Xml xml { get; set; }
            public TranBlock tranBlock { get; set; }
            public TranBlocks tranBlocks { get; set; }
        }

}
}