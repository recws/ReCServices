using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Globalization;

namespace ReCServices.Apis
{
    public class OmnitracsCC
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static ReCServices.WS_OmnitracsCC_Service.vehicleActivityRequestDTO VEHICLEACTREQ = new ReCServices.WS_OmnitracsCC_Service.vehicleActivityRequestDTO();
        public static ReCServices.WS_OmnitracsCC_Service.getPositions ws = new ReCServices.WS_OmnitracsCC_Service.getPositions();
        public static ReCServices.WS_OmnitracsCC_Service.getPositionsResponse wsresp = new ReCServices.WS_OmnitracsCC_Service.getPositionsResponse();

        public static void Omnitracs_ObtenerPosicion(string UsuarioReC, string ClientCode, string Usuario, string Password)
        {
            var responseJson = "";
            try
            {

                var request = new ReCServices.WS_OmnitracsCC_Service.vehicleActivityRequestDTO();
                request.clientCode = ClientCode;
                request.userName = Usuario;
                request.password = Password;
                request.startDate = DateTime.UtcNow.AddMinutes(-30).ToString("yyyy-MM-dd HH:mm:ss"); //"2017-09-20 00:00:00";
                request.endDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"); //"2017-09-20 18:30:00";

                var cliente = new WS_OmnitracsCC_Service.VehicleActivityWebServiceClient();
                //cliente.ClientCredentials.UserName.UserName = "COOPER";
                //cliente.ClientCredentials.UserName.Password = "+61DPoTAES:P";
                var resp = cliente.getPositions(request);
                if (resp.responseDescription == "LOGIN_FAIL : err.ds.SecurityDataService.login.credentials")
                {
                    log.Error("Error LOGIN FAIL " + UsuarioReC + ". "  + "Error de usuario o contraseña.");
                    return;
                }

                if (resp.vehicles == null || resp.vehicles.Count() == 0)
                {
                    //No hay vehiculos asignados
                    return;
                }
                
                for (int i = 0; i < resp.vehicles.Count(); i++)
                {
                    try
                    {
                        var position = resp.vehicles[i].positions.Count();
                        if (position==0)
                        {
                            continue;
                        }
                        //Consulta si ya existe la posicion, por si es repetida y no ha actualizado el equipo
                        string imei = resp.vehicles[i].positions[position - 1].deviceSerial.ToString();
                        string codigoevento = resp.vehicles[i].positions[position - 1].reason.ToString();

                        string lat = resp.vehicles[i].positions[position - 1].latitude.ToString();
                        string lng = resp.vehicles[i].positions[position - 1].longitude.ToString();
                        //string evento = res[i].status.ToString();
                        string odometro = resp.vehicles[i].positions[position - 1].odometer.ToString().Split('.')[0];
                        ////string placas = ((dynamic)res[i]).Plates;
                        string velocidad = resp.vehicles[i].positions[position - 1].speed.ToString().Split('.')[0];
                        string bateria = "100";
                        string direccion = resp.vehicles[i].positions[position - 1].direction.ToString().Split('.')[0];
                        //2017 / 07 / 07 20:37:06
                        //DateTime fechahoragps = DateTime.ParseExact(res[i].T2060.position.posTS);
                        var fechahoragps = resp.vehicles[i].positions[position - 1].posTS.ToUniversalTime();
                        var fechahoraserver = resp.vehicles[i].positions[position - 1].receiveDate.ToUniversalTime();
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
                    catch (Exception Ex)
                    {
                        continue;
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
                    log.Error("Error Omnitracs_ObtenerPosicion: " + UsuarioReC + ". " + responseJson + ". " + Ex.Message);
                }
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
            public object proximity { get; set; }
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
            public List<Omnitracs.Tran> tran { get; set; }
        }

        public class ObjectResult
        {
            public Omnitracs.Xml xml { get; set; }
            public TranBlock tranBlock { get; set; }
        }

    }
}