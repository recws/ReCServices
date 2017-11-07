using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Globalization;
using ReCServices.WS_SEND_OpenSession;
using ReCServices.WS_SEND_GetLastPosition;
using ReCServices.WS_SEND_EventService;
using ReCServices.WS_SEND_StaticService;
using ReCServices.WS_SEND_StateService;
using Token = ReCServices.WS_SEND_EventService.Token;

namespace ReCServices.Apis
{

    public static class Astus
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static WS_SEND_OpenSession.Token Ticket { get; set; }
        public static WS_SEND_StaticService.StaticContractClient StaticServiceClient = new WS_SEND_StaticService.StaticContractClient();
        public static WS_SEND_StateService.StateContractClient StateContractClient = new WS_SEND_StateService.StateContractClient();


        public static string Login(string UsuarioReC, string Usuario, string Password)
        {
            try
            {
                // Obtenir un Ticket avec les informations sur le formulaire
                WS_SEND_OpenSession.OpenSessionRequest RequestToken = new WS_SEND_OpenSession.OpenSessionRequest()
                {
                    Username = Usuario,
                    Password = Password
                };
                WS_SEND_OpenSession.AuthorizationContractClient AuthenticationServiceClient = new WS_SEND_OpenSession.AuthorizationContractClient();
                WS_SEND_OpenSession.OpenSessionResponse ResponseToken = AuthenticationServiceClient.OpenSession(RequestToken);
                if (!ResponseToken.IsAuthenticated)
                {
                    log.Error("Error al obtener token " + UsuarioReC + ". " + "Las credenciales no son validas.");
                    return "Error";
                }
                else
                {
                    log.Info("WebService SEND Autenticado: " + UsuarioReC + ". ");
                    Ticket = ResponseToken.Token;
                    return ResponseToken.IsAuthenticated.ToString();
                }
            }
            catch(Exception Ex)
            {
                //if (Ex.InnerException != null && Ex.InnerException.Message.Contains("404"))
                //{
                //    //No registra nada. cuando da este error es por que no hay ninguna unidad asignada
                //}
                //else
                //{
                    log.Error("Error al obtener token " + UsuarioReC + ". " + Ex.Message);
                //}
            }
            return "Error";
        }
        public static void ObtenereInsertar(string UsuarioReC, string Usuario, string Password)
        {
            try
            {
                if (Ticket == null || Ticket.Value.ToString() == "")
                {
                    string state = Login(UsuarioReC, Usuario, Password);
                    return;
                }

                List<WS_SEND_StaticService.Vehicle> ReturnListVehicles = GetVehicleList(UsuarioReC, Usuario, Password);
                if (ReturnListVehicles == null || ReturnListVehicles.Count == 0)  //No hay unidades colocadas en la cuenta del webervice, es decir, no hay servicio para panalpina
                {
                    return;
                }

                int[] vehicles = ReturnListVehicles.Select(r => r.ID).ToArray();

                // Obtenir la derniere position du véhicule
                WS_SEND_StateService.StateRequest Request = new WS_SEND_StateService.StateRequest()
                {
                    UnsafeToken = GetStateToken(),
                    Vehicles = vehicles
                };
                WS_SEND_StateService.StateContractClient StateServiceClient = new WS_SEND_StateService.StateContractClient();
                WS_SEND_StateService.GetPositionEventResponse Response = StateServiceClient.GetLastPositionEvent(Request);
                if (Response == null)
                    return;
                if (Response.PositionEvents.Length == 0)
                    return;

                if (Response.PositionEvents != null)
                {
                    for (int i = 0; i < Response.PositionEvents.Length; i++)
                    {
                        ////Conversiones de datos
                        var vehicleitem = ReturnListVehicles.SingleOrDefault(x => x.ID == Response.PositionEvents[i].VehicleID);
                        var IMEI = vehicleitem.DeviceID.ToString();
                        var LAT = decimal.Parse(Response.PositionEvents[i].Latitude.ToString());
                        var LNG = decimal.Parse(Response.PositionEvents[i].Longitude.ToString());
                        var GPSValid = Response.PositionEvents[i].IsGPSValid;
                        int ODOMETRO = 0;
                        //var PLACAS = System.Text.RegularExpressions.Regex.Replace(placas, "-", "");
                        //PLACAS = System.Text.RegularExpressions.Regex.Replace(PLACAS, " ", "");
                        var VELOCIDAD = int.Parse(Response.PositionEvents[i].Speed.ToString());
                        var DIRECCION = int.Parse(Response.PositionEvents[i].Heading.ToString());
                        //string[] formats = { "M/dd/yyyy hh:mm:ss tt" };
                        //var dateTime = DateTime.ParseExact(fechahora, formats, new System.Globalization.CultureInfo("en-US"), System.Globalization.DateTimeStyles.None);
                        var dateTime = Response.PositionEvents[i].GPSDateTime;
                        dateTime = dateTime.ToUniversalTime();


                        //Si no es repetida la inserta
                        List<WS_GPS_InsertaSimple_Result> WS_GPS_InsertaSimple;

                        WS_CONTEXT db = new WS_CONTEXT();

                        WS_GPS_InsertaSimple = db.WS_GPS_InsertaSimple(UsuarioReC, IMEI, "1", LAT, LNG, "", GPSValid, VELOCIDAD, DIRECCION, 100, ODOMETRO, dateTime, dateTime).ToList();
                        if (WS_GPS_InsertaSimple[0].Indicador == 1)
                        {

                        }
                        else
                        {
                            var json = JsonConvert.SerializeObject(Response.PositionEvents[0]);
                            log.Error("Error al Insertar evento de " + UsuarioReC + ". IMEI: " + (IMEI != null ? IMEI : "").ToString() + "  -  " + WS_GPS_InsertaSimple[0].Mensaje + ". " + json);
                        }
                    }
                    
                }

            }
            catch (Exception Ex)
            {
                if (Ex.Message.Contains("ticket has expired"))
                {
                    Ticket=null;
                    string state = Login(UsuarioReC, Usuario, Password);
                }
                log.Error("Error al Insertar evento de " + UsuarioReC + ". " + Ex.Message + ". ");
            }
        }


        private static WS_SEND_StateService.Token GetStateToken()
        {
            WS_SEND_StateService.Token ReturnValue = new WS_SEND_StateService.Token();
            ReturnValue.Value = Ticket.Value;
            return ReturnValue;
        }

        private static WS_SEND_StaticService.Token GetStaticToken()
        {
            WS_SEND_StaticService.Token ReturnValue = new WS_SEND_StaticService.Token();
            ReturnValue.Value = Ticket.Value;
            return ReturnValue;
        }

        private static List<WS_SEND_StaticService.Vehicle> GetVehicleList(string UsuarioReC, string Usuario, string Password)
        {
            try
            {
                List<WS_SEND_StaticService.Vehicle> ReturnList = new List<WS_SEND_StaticService.Vehicle>();

                // WebService request
                WS_SEND_StaticService.AuthenticatedRequest Request = new WS_SEND_StaticService.AuthenticatedRequest()
                {
                    UnsafeToken = GetStaticToken()
                };

                WS_SEND_StaticService.GetVehiclesResponse Response = StaticServiceClient.GetVehicles(Request);

                // Verify the response
                if (Response != null)
                    if (Response.Vehicles != null)
                        ReturnList = new List<WS_SEND_StaticService.Vehicle>(Response.Vehicles);

                return ReturnList;
            }
            catch (Exception Ex)
            {
                if (Ex.Message.Contains("ticket has expired"))
                {
                    Ticket = null;
                    string state = Login(UsuarioReC, Usuario, Password);
                }

                log.Error("Error al Consultar el listado de unidades " + UsuarioReC + ". " + Ex.Message);
                return null;
            }
        }
    }
}