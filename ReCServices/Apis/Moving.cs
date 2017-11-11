using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using System.Xml;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
//using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using System.Xml.Serialization;
using System.Web.Services;


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
    
                
                WS_Moving_Service.DoLoginRequest loginRequest = new WS_Moving_Service.DoLoginRequest();
                WS_Moving_Service.ServiceSoapClient cliente = new WS_Moving_Service.ServiceSoapClient();

                loginRequest.UserCredential = new WS_Moving_Service.UserCredentialInfo();
                loginRequest.UserCredential.UserName = Usuario;
                loginRequest.UserCredential.Password = Password;
                loginRequest.Session = new WS_Moving_Service.SessionInfo();

                WS_Moving_Service.DoLoginResponse loginResponse = null;
                loginResponse = cliente.DoLogin(loginRequest);

                if (loginResponse != null && loginResponse.OperationStatus || loginResponse.Authenticated)
                {  
                    WS_Moving_Service.GetVehiclesRequest vehiclesreq = new WS_Moving_Service.GetVehiclesRequest();
                    vehiclesreq.Session = loginResponse.SecurityProfile.Session;
                    vehiclesreq.IsProfile = false;
                    vehiclesreq.Version = 0;
                    vehiclesreq.OwnerId = loginResponse.SecurityProfile.User.OwnerID;

                    var vehicles = cliente.GetVehicles(vehiclesreq);

                    if (vehicles.Vehicles == null || vehicles.Vehicles.Length == 0) {
                        return;  //no hay vehiculos asignados a la cuenta
                    }

                    WS_Moving_Service.GetVehicleSnapShotsRequest snapshotsreq = new WS_Moving_Service.GetVehicleSnapShotsRequest();
                    snapshotsreq.Session = loginResponse.SecurityProfile.Session;
                    snapshotsreq.Version = 0;
                    snapshotsreq.OwnerId = loginResponse.SecurityProfile.User.OwnerID;
                    var snapshots = cliente.GetVehicleSnapShots(snapshotsreq);

                    for (int i = 0; i < vehicles.Vehicles.Length; i++)
                    {
                        var uniquevehicleid = vehicles.Vehicles[i].UniqueVehicleID;
                        var vehicleid = vehicles.Vehicles[i].VehicleId;
                        var snapshot = snapshots.SnapShots.Where(x => x.VehicleId == vehicleid).ToList();

                        //Consulta si ya existe la posicion, por si es repetida y no ha actualizado el equipo
                        string imei = uniquevehicleid;
                        string codigoevento = snapshot[0].EventSubType != null && snapshot[0].EventSubType.ToString() !="" ? snapshot[0].EventSubType.ToString().Replace("SMDP_EVENT_", "") : "";
                        codigoevento = codigoevento.Substring(0, codigoevento.Length >= 29 ? 29 : codigoevento.Length);

                        string lat = snapshot[0].Latitude.ToString();
                        string lng = snapshot[0].Longitude.ToString();
                        //string evento = res[i].status.ToString();
                        string odometro = snapshot[0].ODOMeter.ToString().Split('.')[0];
                        string placas = vehicles.Vehicles[i].Registration;
                        string velocidad = snapshot[0].Speed.ToString().Split('.')[0];
                        string bateria = "100";
                        string direccion = snapshot[0].HDG.ToString().Split('.')[0];
                        var fechahoragps = snapshot[0].ActivityDateTime;
                        var fechahoraserver = snapshot[0].ActivityDateTime;
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

                    //Cerrar sesion al finalizar:
                    WS_Moving_Service.DoLogoffRequest logoffRequest = new WS_Moving_Service.DoLogoffRequest();
                    logoffRequest.Session = loginResponse.SecurityProfile.Session;
                    WS_Moving_Service.DoLogoffResponse logoffResponse = null;
                    logoffResponse = cliente.DoLogoff(logoffRequest);
                }
                else {
                    return;
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

        public static HttpWebRequest CreateWebRequest()
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(@"https://onlineavl2api-mx02.navmanwireless.com/onlineavl/api/V1.0/service.asmx?wsdl");
            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }
    }
}