using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Globalization;


namespace ReCServices.Apis
{
    public class Moving
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    

        public static void Moving_ObtenerPosicion(string UsuarioReC, string Usuario, string Password)
        {
            var responseJson = "";
            try
            {
                //WS_Moving_Service.BaseMessage bm = new WS_Moving_Service.BaseMessage();
                //WS_Moving_Service.BaseMessageRequest bmr = new WS_Moving_Service.BaseMessageRequest();
                WS_Moving_Service.DoLoginRequest dologin = new WS_Moving_Service.DoLoginRequest();
                WS_Moving_Service.ServiceSoapClient cliente = new WS_Moving_Service.ServiceSoapClient();

                dologin.UserCredential = new WS_Moving_Service.UserCredentialInfo();
                dologin.UserCredential.UserName = "api_moving";
                dologin.UserCredential.Password = "p83XAfHG";
                dologin.UserCredential.ApplicationID = new Guid();
                dologin.UserCredential.ClientID = new Guid();
                dologin.UserCredential.ClientVersion = "0";
                dologin.Session = new WS_Moving_Service.SessionInfo();
                dologin.Session.SessionId = new Guid();
                var x = cliente.DoLogin(dologin);

                var state = cliente.State;

                WS_Moving_Service.GetVehiclesRequest vr = new WS_Moving_Service.GetVehiclesRequest();
                vr.Session = x.SecurityProfile.Session;
                vr.IsProfile = true;
                vr.Version = 0;
                vr.OwnerId = x.SecurityProfile.User.OwnerID;
                
                var vehicles = cliente.GetVehicles(vr);

                WS_Moving_Service.GetVehicleSnapShotsRequest vreq = new WS_Moving_Service.GetVehicleSnapShotsRequest();
                vreq.OwnerId = x.SecurityProfile.User.OwnerID;
                vreq.Session = x.SecurityProfile.Session;
                vreq.Version = 0;
                var vehicle = cliente.GetVehicleSnapShots(vreq);
                WS_Moving_Service.GetVehiclesActivityDetailsRequest vadr = new WS_Moving_Service.GetVehiclesActivityDetailsRequest();
                vadr.Session = x.SecurityProfile.Session;
                var guid = new Guid[1];
                guid[0] = vehicles.Vehicles[0].VehicleId;
                vadr.ActivityLogIDs = guid;
                var activitylogs = cliente.GetVehiclesActivityDetails(vadr);

                return;

                //var xmlstring = "";
                ////var x = ws.getSubscriberInfo(1);
                //if (UsuarioReC == "WS_Masetto")
                //{
                //    var dequeue = ws.dequeue2(1, 1);
                //    xmlstring = System.Text.Encoding.Default.GetString(dequeue.transactions);
                //}
                //else if (UsuarioReC == "WS_Jaguar")
                //{

                //    var dequeue = ws_jaguar.dequeue2(1, 1);
                //    xmlstring = System.Text.Encoding.Default.GetString(dequeue.transactions);
                //}
                //else
                //{
                //    return;
                //}

                //if (xmlstring == "")
                //{
                //    return;
                //}
                //System.Xml.XmlDocument doc2 = new System.Xml.XmlDocument();
                //doc2.LoadXml(xmlstring);
                //string json2 = JsonConvert.SerializeXmlNode(doc2);
                //json2 = System.Text.RegularExpressions.Regex.Replace(json2, "string", "Dataset");
                //json2 = json2.Replace("?", "");
                //json2 = json2.Replace("#", "");
                //json2 = System.Text.RegularExpressions.Regex.Replace(json2, "@", "");
                //json2 = json2.Replace("T.2.06.0", "T2060");

                //var result = JsonConvert.DeserializeObject<ObjectResult>(json2);
                //var res = result.tranBlock.tran;

                //for (int i = 0; i < res.Count; i++)
                //{
                //    try
                //    {
                //        //Consulta si ya existe la posicion, por si es repetida y no ha actualizado el equipo
                //        string imei = res[i].T2060.equipment.unitAddress.ToString();
                //        string codigoevento = "";

                //        string lat = res[i].T2060.position.lat;
                //        string lng = res[i].T2060.position.lon;
                //        //string evento = res[i].status.ToString();
                //        string odometro = "0";
                //        ////string placas = ((dynamic)res[i]).Plates;
                //        string velocidad = "0";
                //        string bateria = "100";
                //        string direccion = "0";
                //        //2017 / 07 / 07 20:37:06
                //        //DateTime fechahoragps = DateTime.ParseExact(res[i].T2060.position.posTS);
                //        var fechahoragps = DateTime.ParseExact(res[i].T2060.position.posTS, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
                //        fechahoragps = fechahoragps.ToUniversalTime();
                //        ////Validaciones

                //        //////Conversiones de datos
                //        var LAT = decimal.Parse(lat);
                //        var LNG = decimal.Parse(lng);
                //        var ODOMETRO = int.Parse(odometro);
                //        ////var PLACAS = System.Text.RegularExpressions.Regex.Replace(placas, "-", "");
                //        ////PLACAS = System.Text.RegularExpressions.Regex.Replace(PLACAS, " ", "");
                //        var VELOCIDAD = int.Parse(velocidad);
                //        var DIRECCION = int.Parse(direccion);
                //        var BATERIA = int.Parse(bateria);


                //        //Si no es repetida la inserta
                //        List<WS_GPS_InsertaSimple_Result> WS_GPS_InsertaSimple;

                //        WS_CONTEXT db = new WS_CONTEXT("WS_CONTEXT_PROD");

                //        WS_GPS_InsertaSimple = db.WS_GPS_InsertaSimple(UsuarioReC, imei, codigoevento, LAT, LNG, "", true, VELOCIDAD, DIRECCION, BATERIA, ODOMETRO, fechahoragps, fechahoragps).ToList();
                //        if (WS_GPS_InsertaSimple[0].Indicador == 1)
                //        {

                //        }
                //        else
                //        {
                //            log.Error("Error al Insertar evento de " + UsuarioReC + ". " + " IMEI: " + imei + ". " + WS_GPS_InsertaSimple[0].Mensaje + ". ");
                //        }
                //    }
                //    catch (Exception Ex)
                //    {
                //        continue;
                //    }
                //}


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
    }
}