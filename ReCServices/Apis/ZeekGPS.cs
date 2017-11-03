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
    public static class ZeekGPS
    {
      

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static string Token { get; set; }
        public static ReCServices.WS_ZeekGPS_Service.CMovilApi3WSSoapClient ws = new ReCServices.WS_ZeekGPS_Service.CMovilApi3WSSoapClient();
        public static ReCServices.WS_ZeekGPS_Service.UsuarioAutorizado auth;


        public static void ZeekGPS_ObtenerPosicion(string UsuarioReC, string Usuario, string Password, string Licencia)
        {
            var responseJson = "";
            try
            {
                auth = ws.AutentificaUsuario(Usuario, Password, Licencia);
                Token = auth.Token.ToString();
                if (Token == "0")
                {
                    log.Info("WebService ZeekGPS Error de Autenticacion: " + UsuarioReC + ". ");
                    return;
                }
                else {
                    try
                    {
                        var listaunidades = auth.Vehiculos.Select(x => x.Unidad).ToArray();  //Se filtra unicamente la columna unidad para pasarlo como parametro adelante
                        var result = ws.UltimasUbicaciones(long.Parse(Token), Licencia, auth.Cliente, listaunidades);

                        if (listaunidades.Count() == 0 || result == null)  //No hay unidades en el webservice
                        {
                            return;
                        }


                        var vehiculos = result.ToList();
                        for (int i = 0; i < vehiculos.Count(); i++)
                        {
                            try
                            {
                                //Consulta si ya existe la posicion, por si es repetida y no ha actualizado el equipo
                                string imei = vehiculos[i].Unidad.ToString();
                                string codigoevento = "1";

                                string lat = LatLngNumeroaDecimal(vehiculos[i].Latitud).ToString().Substring(0, vehiculos[i].Latitud.ToString().Length >= 12 ? 12 : vehiculos[i].Latitud.ToString().Length);
                                string lng = LatLngNumeroaDecimal(vehiculos[i].Longitud).ToString().Substring(0, vehiculos[i].Longitud.ToString().Length >= 14 ? 14 : vehiculos[i].Longitud.ToString().Length);
                                //string evento = result[i].status.ToString();
                                string odometro = vehiculos[i].defOdometro.ToString();
                                ////string placas = ((dynamic)res[i]).Plates;
                                string velocidad = vehiculos[i].Velocidad.ToString().Split('.')[0];
                                string bateria = "100";
                                string direccion = "0";

                                var fechahoragps = vehiculos[i].Fecha;
                                //fechahoragps = fechahoragps.ToUniversalTime();

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
                                    log.Error("Error al Insertar evento de " + UsuarioReC + ". IMEI: " + (imei != null ? imei : "").ToString() + "  -  " + WS_GPS_InsertaSimple[0].Mensaje + ". " + responseJson);
                                }
                            }
                            catch (Exception Ex)
                            {
                                log.Error("Error ZeekGPS_ObtenerPosicion: " + UsuarioReC + ". " + responseJson + ". " + Ex.Message);
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
                            log.Error("Error ZeekGPS_ObtenerPosicion: " + UsuarioReC + ". " + responseJson + ". " + Ex.Message);
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
                    log.Error("Error ZeekGPS_ObtenerToken: " + UsuarioReC + ". " + Ex.Message);
                }
                //Token = "";
            }

            
        }

        public static decimal LatLngNumeroaDecimal(int LatorLng)
        {
            try
            {
                //Obtener el signo + o -, quitarselo y al final ponerselo
                int signo = LatorLng >= 0 ? 1 : -1;
                string ValorAbsoluto = Math.Abs(LatorLng).ToString();
                //7 numeros es latitud, 8 numeros es longitud
                int totnumeros = ValorAbsoluto.Length;
                int totnumhoras = totnumeros == 8 ? 3 : 2;
                //extrae los primeros dos digitos
                var horas = int.Parse(ValorAbsoluto.Substring(0, totnumhoras));  //Toma los primeros dos o tres numeros dependiendo la longitud de la cadena
                var minutos = decimal.Parse(ValorAbsoluto.Substring(totnumhoras, 2)) / 60;
                var segundos = (decimal.Parse("." + ValorAbsoluto.Substring(totnumhoras+2)) * 60) / 3600;

                var res = horas + minutos + segundos;
                return res * signo;
            }
            catch(Exception Ex)
            {
                return 0;
            }
        }

    }


}