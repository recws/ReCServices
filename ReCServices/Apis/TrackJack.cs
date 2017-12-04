using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace ReCServices.Apis
{
    public class TrackJack
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Trackjack_ObtenerPosicion(string UsuarioReC, string Usuario, string Password)
        {
            var responseJson = "";
            try
            {

                WS_Trackjack_Service.ServiceSoapClient servicePosicionesTodos = new WS_Trackjack_Service.ServiceSoapClient();
                var vehicles = servicePosicionesTodos.PosicionesTodos(Usuario, Password);

                for (int i = 0; i < vehicles.Length; i++)
                {
                    try
                    {
                        string imei = vehicles[i].UnitPlate.ToString();
                        imei = System.Text.RegularExpressions.Regex.Replace(imei, "-", "");
                        imei = System.Text.RegularExpressions.Regex.Replace(imei, " ", "");

                        if(imei == "S/N")
                        {
                            continue;
                        }

                        string codigoevento = vehicles[i].Evento.ToString();

                        string lat = vehicles[i].Latitude.ToString();
                        string lng = vehicles[i].Longitude.ToString();
                        string odometro = vehicles[i].Odometer.ToString().Split('.')[0];
                        ////string placas = ((dynamic)res[i]).Plates;
                        string velocidad = vehicles[i].SpeedGps.ToString().Split('.')[0];

                        string bateria = "100";
                        string direccion = vehicles[i].direccion.ToString().Split('.')[0];


                        var fechahoragps = DateTime.ParseExact(vehicles[i].DateGps, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                        fechahoragps = fechahoragps.ToUniversalTime();
                        var fechahoraserver = DateTime.ParseExact(vehicles[i].DateGps, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                        fechahoraserver = fechahoraserver.ToUniversalTime();

                        //////Conversiones de datos
                        ////codigoevento = codigoevento;
                        ////evento = evento;
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
                    catch (Exception Ex)
                    {   
                        log.Error("Error GrupoUDA_ObtenerPosicion: " + UsuarioReC + ". " + Ex.Message);                        
                    }
                }
            }
            catch (Exception Ex)
            {
                log.Error("Error Trackjack_ObtenerPosicion: " + UsuarioReC + ". " + responseJson + ". " + Ex.Message);                
            }
        }
    }
}