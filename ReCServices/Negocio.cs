using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Microsoft.AspNet.SignalR;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Net.Mail;

namespace ReCServices
{
    public class Negocio
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public RespuestaServicio InsertaSimple(EventoSimple eventosimple, string ServicioOrigen="")
        {
            RespuestaServicio respuesta = new RespuestaServicio();
            string DomainName = "";
            try
            {
                DomainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            }
            catch (Exception Ex)
            {
                DomainName = "";
            }


            //Validaciones de datos

            respuesta.Indicador = 0;
            if (eventosimple.Usuario == "")
            {
                respuesta.Mensaje = "Error: El Usuario no puede estar vacio";
                var json = JsonConvert.SerializeObject(eventosimple);
                log.Error(respuesta.Mensaje + " - " + json);
                return respuesta;
            }
            if (eventosimple.Password == "")
            {
                respuesta.Mensaje = "Error: El Password no puede estar vacio";
                var json = JsonConvert.SerializeObject(eventosimple);
                log.Error(respuesta.Mensaje + " - " + json);
                return respuesta;
            }
            //if ((eventosimple.IdGrupo > 0) == false)
            //{
            //    respuesta.Mensaje = "Error: Debe indicar el IdGrupo que le asignaron";
            //    return respuesta;
            //}
            if (eventosimple.IMEI == "")
            {
                respuesta.Mensaje = "Error: El IMEI no puede estar vacio";
                var json = JsonConvert.SerializeObject(eventosimple);
                log.Error(respuesta.Mensaje + " - " + json);
                return respuesta;
            }
            //if ((eventosimple.Lat > 0) == false)
            //{
            //    respuesta.Mensaje = "Error: Latitud incorrecta";
            //    var json = JsonConvert.SerializeObject(eventosimple);
            //    log.Error(respuesta.Mensaje + " - " + json);
            //    return respuesta;
            //}
            //if (eventosimple.Lng == 0)
            //{
            //    respuesta.Mensaje = "Error: Longitud incorrecta";
            //    var json = JsonConvert.SerializeObject(eventosimple);
            //    log.Error(respuesta.Mensaje + " - " + json);
            //    return respuesta;
            //}
            if (eventosimple.FechaHoraRecepcion <= DateTime.Today.AddDays(-3))
            {
                respuesta.Mensaje = "Error: La fecha es antigua, solo se permiten eventos recientes.";
                return respuesta;
            }



            //Excepciones
            if (eventosimple.IMEI == "530ER7" && eventosimple.Usuario != "WS_Marloz")
            {
                eventosimple.Usuario = "WS_Marloz";
                eventosimple.Password = "xwKNgQyp";
            }


            //Si pasaron las validaciones intenta hacer el Insert
            try
            {
                Result result = ServicioOrigen == "RestApi" ? ValidaUsuario(eventosimple) : ValidaUsuarioyPassword(eventosimple);


                if (result.Indicador == 1)
                {

                    List<WS_GPS_InsertaSimple_Result> WS_GPS_InsertaSimple;

                    WS_CONTEXT db = new WS_CONTEXT();

                    WS_GPS_InsertaSimple = db.WS_GPS_InsertaSimple(eventosimple.Usuario, eventosimple.IMEI, eventosimple.CodigoEvento, eventosimple.Lat, eventosimple.Lng, eventosimple.Ubicacion, eventosimple.GPSValido, eventosimple.Velocidad, eventosimple.Direccion, eventosimple.NivelBateria, eventosimple.KMOdometro, eventosimple.FechaHoraGeneracion, eventosimple.FechaHoraRecepcion).ToList();
                    if (WS_GPS_InsertaSimple[0].Indicador == 1)
                    {
                        //Pasa el dato por socket a los clientes conectados solo si es en produccion
                        //if (DomainName.Contains("ws.") || DomainName.TrimEnd() == "http://ws.recsolutions.tech" )
                        //{
                            var context = GlobalHost.ConnectionManager.GetHubContext<GPSHub>();
                            context.Clients.All.broadCastMessage("Server", "GPS", eventosimple);
                        //}

                    }

                    if (WS_GPS_InsertaSimple[0].Indicador == 1)
                    {
                        respuesta.Indicador = 1;
                        respuesta.Mensaje = "OK";
                        return respuesta;
                    }
                    else
                    {
                        respuesta.Indicador = 0;
                        respuesta.Mensaje = "Error: " + WS_GPS_InsertaSimple[0].Mensaje;
                        var json = JsonConvert.SerializeObject(eventosimple);
                        log.Error(respuesta.Mensaje + " - " + json);
                        return respuesta;
                    }
                }
                else
                {
                    respuesta.Indicador = 0;
                    respuesta.Mensaje = "Error: " + result.Mensaje;
                    var json = JsonConvert.SerializeObject(eventosimple);
                    log.Error(respuesta.Mensaje + " - " + json);
                    return respuesta;
                }
            }
            catch (Exception Ex)
            {
                respuesta.Indicador = 0;
                respuesta.Mensaje = "Error: " + Ex.Message;
                var json = JsonConvert.SerializeObject(eventosimple);
                log.Error(respuesta.Mensaje + " - " + json);
                return respuesta;
            }

        }

        private Result ValidaUsuario(EventoSimple eventosimple)
        {

            WS_CONTEXT db = new WS_CONTEXT();

            Result result = new Result();

            //Obtener el IdUsuario en base a Usuario y Contraseña y validarlo
            var resultado = db.WS_GPS_ValidaUsuario(eventosimple.Usuario, eventosimple.IMEI).ToList();
            result.Indicador = resultado[0].Indicador;
            result.Mensaje = resultado[0].Mensaje;
            result.IdRegistroAfectado = (int)resultado[0].IdRegistroAfectado;
            
            return result;

        }
        private Result ValidaUsuarioyPassword(EventoSimple eventosimple)
        {

            WS_CONTEXT db = new WS_CONTEXT();

            Result result = new Result();

            //Obtener el IdUsuario en base a Usuario y Contraseña y validarlo
            var resultado = db.WS_GPS_ValidaUsuarioyPassword(eventosimple.Usuario, eventosimple.Password, eventosimple.IMEI).ToList();
            result.Indicador = resultado[0].Indicador;
            result.Mensaje = resultado[0].Mensaje;
            result.IdRegistroAfectado = (int)resultado[0].IdRegistroAfectado;

            return result;

        }

        public void WS_GPS_SincronizaUsuario()
        {

            WS_CONTEXT db = new WS_CONTEXT("WS_CONTEXT_PROD");
            db.WS_GPS_SincronizaUsuario();

            WS_CONTEXT dbTEST = new WS_CONTEXT("WS_CONTEXT_TEST");
            dbTEST.WS_GPS_SincronizaUsuario();

            //EnviarCorreo("olopez@gccomerce.com", "", "", "Ejecucion Quartz", "se ejecuto la tarea sincronizar usuario");

        }

        public void WS_GPS_SincronizaGPS()
        {

            WS_CONTEXT db = new WS_CONTEXT("WS_CONTEXT_PROD");
            db.WS_GPS_SincronizaGPS();

            WS_CONTEXT dbTEST = new WS_CONTEXT("WS_CONTEXT_TEST");
            dbTEST.WS_GPS_SincronizaGPS();

            //EnviarCorreo("olopez@gccomerce.com", "", "", "Ejecucion Quartz", "se ejecuto la tarea sincronizar gps");

        }

        public class Result
        {
            public int Indicador { get; set; }
            public string Mensaje { get; set; }
            public int IdRegistroAfectado { get; set; }
        }


        public void EnviarCorreo(string Destinatarios_Para, string Destinatarios_CC, string Destinatarios_CCO, string Asunto, string Cuerpo)
        {
            try
            {

                SmtpClient smtpServer = new SmtpClient();
                MailMessage mailMessage = new MailMessage();

                mailMessage.To.Add(FormatMultipleEmailAddresses(Destinatarios_Para));

                if (!string.IsNullOrEmpty(Destinatarios_CC))
                {
                    mailMessage.CC.Add(FormatMultipleEmailAddresses(Destinatarios_CC));
                }
                if (!string.IsNullOrEmpty(Destinatarios_CCO))
                {
                    MailAddress bcc = new MailAddress("");
                    mailMessage.Bcc.Add(bcc);
                }


                    smtpServer.Host = "mail.gccomerce.com";
                    smtpServer.UseDefaultCredentials = true;
                    smtpServer.Port = 25;
                    smtpServer.EnableSsl = false;
                    smtpServer.Credentials = new System.Net.NetworkCredential("notificaciones@gccomerce.com", "n_321");
                    //mailMessage.From = new MailAddress("reportes@recsolutions.tech", "Notificaciones ReC Technology", System.Text.Encoding.UTF8);
                    mailMessage.From = new MailAddress("notificaciones@gccomerce.com", "Notificaciones ReC", System.Text.Encoding.UTF8);

                DateTime FechaActual = DateTime.Now;
                mailMessage.Subject = Asunto + " " + FechaActual.Day.ToString("00") + "-" + FechaActual.Month.ToString("00") + "-" + FechaActual.Year.ToString() + " " + FechaActual.Hour.ToString("00") + ":" + FechaActual.Minute.ToString("00") + " HRS";





                mailMessage.Body = Cuerpo;
                mailMessage.IsBodyHtml = true;


                mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                smtpServer.Send(mailMessage);

                mailMessage.Attachments.Dispose();
                mailMessage.Dispose();
                smtpServer.Dispose();


            }
            catch (SmtpException error)
            {

                string mensaje = "Error: ";
                try
                {
                    mensaje = mensaje + " " + error.Message;
                    mensaje = mensaje + " " + error.InnerException.Message;
                }
                catch (Exception Ex) {
                    log.Error(Ex.Message);
                }
                string errorsString = JsonConvert.SerializeObject(error);
                log.Error(mensaje + " " + errorsString);
                throw new HttpException(mensaje + " " + errorsString);

            }
            catch (Exception error)
            {

                string mensaje = "Error: ";
                try
                {
                    mensaje = mensaje + " " + error.Message;
                    mensaje = mensaje + " " + error.InnerException.Message;
                }
                catch (Exception Ex) {
                    log.Error(Ex.Message);
                }
                string errorsString = JsonConvert.SerializeObject(error);
                log.Error(mensaje + " " + errorsString);
                throw new HttpException(mensaje + " " + errorsString);
            }


        }
        public static string FormatMultipleEmailAddresses(string emailAddresses)
        {
            var delimiters = new[] { ',', ';' };

            var addresses = emailAddresses.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            return string.Join(",", addresses);
        }

        public void ReportexCorreoLogLleno()
        {
            DataTable DT_Data;

            DT_Data = GetData_LogLleno("Reporte_LogLleno");
            if (DT_Data.Rows.Count > 0)
            {
                EnviarCorreo("olopez@gccomerce.com", "", "", "Log de base de datos lleno", "Urgente revisar el log de base de datos WS PROD " + DT_Data.Rows[0]["SizeMB"].ToString() + " mb");
            }

        }
        public static DataTable GetData_LogLleno(string StoredProcedure)
        {
            DataTable dtbl = new DataTable("DataTable1");

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WS_PROD"].ConnectionString);
            SqlDataAdapter adp = new SqlDataAdapter();
            adp.SelectCommand = new SqlCommand();
            adp.SelectCommand.Connection = con;

            adp.SelectCommand.CommandText = StoredProcedure;
            adp.SelectCommand.CommandType = CommandType.StoredProcedure;
            adp.Fill(dtbl);

            return dtbl;
        }

        public static DataTable WS_GPS_ConsultaIncidenciasAutomaticas(string Usuario)
        {
            DataTable dtbl = new DataTable("DataTable1");

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WS_PROD"].ConnectionString);
            SqlDataAdapter adp = new SqlDataAdapter();
            adp.SelectCommand = new SqlCommand();
            adp.SelectCommand.Connection = con;

            adp.SelectCommand.CommandText = "WS_GPS_ConsultaIncidenciasAutomaticas";
            adp.SelectCommand.CommandType = CommandType.StoredProcedure;
            adp.SelectCommand.Parameters.AddWithValue("@Usuario", Usuario);
            adp.Fill(dtbl);

            return dtbl;
        }
        public static DataTable WS_GPS_EliminaIncidencia(int IdEventoGPS)
        {
            DataTable dtbl = new DataTable("DataTable1");

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WS_PROD"].ConnectionString);
            SqlDataAdapter adp = new SqlDataAdapter();
            adp.SelectCommand = new SqlCommand();
            adp.SelectCommand.Connection = con;

            adp.SelectCommand.CommandText = "WS_GPS_EliminaIncidencia";
            adp.SelectCommand.CommandType = CommandType.StoredProcedure;
            adp.SelectCommand.Parameters.AddWithValue("@IdEventoGPS", IdEventoGPS);
            adp.Fill(dtbl);

            return dtbl;
        }
        public void NotificacionIncidencia()
        {
            DataTable DT_Data;

            DT_Data = WS_GPS_ConsultaIncidenciasAutomaticas("WS_EasyTrack");

            for (int i = 0; i < DT_Data.Rows.Count; i++)
            {


                //Obtiene ahora el html del correo y contactos
                Monitoreo db = new Monitoreo();
                List<NotificacionIncidencia_CargaHTML_Result> HTML;
                DataRow GPS = DT_Data.Rows[i];
                HTML = db.NotificacionIncidencia_CargaHTML(DT_Data.Rows[i]["IMEI"].ToString(), "WS_EasyTrack", DT_Data.Rows[i]["Incidencia"].ToString()).ToList();
                if (HTML.Count() == 1)
                {
                    var Asunto = ReemplazaTexto(GPS, HTML, HTML[0].Asunto.ToString());
                    var Cuerpo = ReemplazaTexto(GPS, HTML, HTML[0].Cuerpo.ToString());

                    //Envia el correo
                    EnviarCorreo(HTML[0].Para.ToString(), HTML[0].CC.ToString(), HTML[0].CCO.ToString(), Asunto, Cuerpo);

                    //Elimina el registro de la tabla
                    WS_GPS_EliminaIncidencia((int)GPS["IdEventoGPS"]);
                }
            }
        }

        private string ReemplazaTexto(DataRow GPS, List<NotificacionIncidencia_CargaHTML_Result> HTML, string texto)
        {
            try
            {
                texto = System.Text.RegularExpressions.Regex.Replace(texto, "@GRUPO", HTML[0].Grupo == null ? "" : HTML[0].Grupo);
                texto = System.Text.RegularExpressions.Regex.Replace(texto, "@OPERACION", HTML[0].Operacion == null ? "" : HTML[0].Operacion);
                texto = System.Text.RegularExpressions.Regex.Replace(texto, "@CLASIFICACION", "Incidencia");
                texto = System.Text.RegularExpressions.Regex.Replace(texto, "@CONTROLADO", "Critico");
                texto = System.Text.RegularExpressions.Regex.Replace(texto, "@TIPOINCIDENCIA", "");
                texto = System.Text.RegularExpressions.Regex.Replace(texto, "@INCIDENCIA", HTML[0].Incidencia == null ? "" : HTML[0].Incidencia);
                //texto = System.Text.RegularExpressions.Regex.Replace(texto, "@FECHAHORA",((DateTime)GPS["FechaHora"]).ToString("dd-MM-yyyy HH:mm"));
                //texto = System.Text.RegularExpressions.Regex.Replace(texto, "@COLOR", HTML[0].Grupo);
                texto = System.Text.RegularExpressions.Regex.Replace(texto, "@ESTATUSVIAJE", HTML[0].EstatusViaje == null ? "" : HTML[0].EstatusViaje);
                texto = System.Text.RegularExpressions.Regex.Replace(texto, "@OBSERVACIONES", HTML[0].Observaciones == null ? "" : HTML[0].Observaciones);
                texto = System.Text.RegularExpressions.Regex.Replace(texto, "@FOLIOCLIENTE", HTML[0].FolioCliente == null ? "" : HTML[0].FolioCliente);
                texto = System.Text.RegularExpressions.Regex.Replace(texto, "@FOLIOINTERNO", HTML[0].FolioInterno == null ? "" : HTML[0].FolioInterno);
                texto = System.Text.RegularExpressions.Regex.Replace(texto, "@OPERADOR", HTML[0].Operador == null ? "" : HTML[0].Operador);
                texto = System.Text.RegularExpressions.Regex.Replace(texto, "@UNIDAD", HTML[0].Unidad == null ? "" : HTML[0].Unidad);
                texto = System.Text.RegularExpressions.Regex.Replace(texto, "@PLACAS", HTML[0].Placas == null ? "" : HTML[0].Placas);
                texto = System.Text.RegularExpressions.Regex.Replace(texto, "@TRANSPORTISTA", HTML[0].Transportista == null ? "" : HTML[0].Transportista);
                texto = System.Text.RegularExpressions.Regex.Replace(texto, "@DESTINOS", HTML[0].Destinos == null ? " < strong > Sin Destinos </ strong > " + " < br >< br > " : HTML[0].Destinos + "<br><br>");
                texto = System.Text.RegularExpressions.Regex.Replace(texto, "@TIPOVIAJE", HTML[0].TipoViaje == null ? "" : HTML[0].TipoViaje);
                texto = System.Text.RegularExpressions.Regex.Replace(texto, "@AGENCIAADUANAL", HTML[0].AgenciaAduanal == null ? "" : "<strong>IMPO-EXPO: </strong>" + HTML[0].AgenciaAduanal + "  " + "<br>");
                texto = System.Text.RegularExpressions.Regex.Replace(texto, "@FORWARDING", HTML[0].Forwarding == null ? "" : "<strong>Forwarding: </strong>" + HTML[0].Forwarding + "  " + "<br>");
                texto = System.Text.RegularExpressions.Regex.Replace(texto, "@CONTENEDOR1", HTML[0].Contenedor1 == null ? "" : "<strong><u>CONTENEDOR 1:</u></strong> " + HTML[0].Contenedor1 + " <br>");
                texto = System.Text.RegularExpressions.Regex.Replace(texto, "@CONTENEDOR2", HTML[0].Contenedor2 == null ? "" : "<strong><u>CONTENEDOR 2:</u></strong> " + HTML[0].Contenedor2 + " <br><br>");

                //texto = System.Text.RegularExpressions.Regex.Replace(texto, "@UBICACION", HTML[0].Grupo.ToString());
                texto = System.Text.RegularExpressions.Regex.Replace(texto, "@COORDENADAS", GPS["Lat"].ToString() + " " + GPS["Lng"].ToString());
                texto = System.Text.RegularExpressions.Regex.Replace(texto, "@VELOCIDAD", GPS["Velocidad"].ToString());
                //texto = System.Text.RegularExpressions.Regex.Replace(texto, "@DISTANCIADESTINO", HTML[0].Grupo);

                string mapa = "Imágen de mapa:" +
                            "<br>" +
                            "<a alt='Google Maps' href='https://maps.google.com/?q=" + GPS["Lat"].ToString() + " " + GPS["Lng"].ToString() + "'>" +
                                "<img src='https://maps.google.com/maps/api/staticmap?size=480x300&markers=color:red|" + GPS["Lat"].ToString() + " " + GPS["Lng"].ToString() + "&sensor=true' />" +
                            "</a>" +
                            "<br>";

                texto = System.Text.RegularExpressions.Regex.Replace(texto, "@MAPA", mapa);


                texto = System.Text.RegularExpressions.Regex.Replace(texto, "@DISTANCIADESTINO", "");
                texto = System.Text.RegularExpressions.Regex.Replace(texto, "@UBICACION", "");

            }
            catch (Exception Ex)
            {
                log.Error("Error al reemplazar las variables. " + Ex.Message);
            }
            return texto;
        }

        public void TruncarReCLog()
        {
            string Estatus = "";
            string sPath = "";
            sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/")+ "ReC.log";

            long filelength = new System.IO.FileInfo(sPath).Length / 1024;
            if (filelength >= 5000)
            {
                //Copia el archivo con otro nombre

                try
                {
                    try
                    {
                        System.IO.File.Copy(sPath, System.Web.Hosting.HostingEnvironment.MapPath("~/") + "ReC_" + DateTime.Now.ToString("ddMMyyyyHHmm") + ".log");
                    }
                    catch (Exception Ex)
                    {
                    }

                    
                    try
                    {
                        using (var fs = new System.IO.FileStream(sPath, System.IO.FileMode.Truncate))
                        {
                        }
                        Estatus += "Truncado 1 correcto ReC.Log" + "(" + filelength.ToString() + ")";
                    }
                    catch (Exception Ex)
                    {
                        Estatus += "Truncado 1 error ReC.Log" + "(" + filelength.ToString() + ")";
                    }

                    try
                    {
                        System.IO.File.WriteAllBytes(sPath, new byte[0]);
                        Estatus += "Truncado 2 correcto ReC.Log" + "(" + filelength.ToString() + ")";
                    }
                    catch (Exception Ex)
                    {
                        Estatus += "Truncado 2 error ReC.Log" + "(" + filelength.ToString() + ")";
                    }

                    try
                    {
                        System.IO.File.WriteAllText(sPath, string.Empty);
                        Estatus += "Truncado 3 correcto ReC.Log" + "(" + filelength.ToString() + ")";
                    }
                    catch (Exception Ex)
                    {
                        Estatus += "Truncado 3 error ReC.Log" + "(" + filelength.ToString() + ")";
                    }

                    try
                    {
                        System.IO.FileStream fs = new System.IO.FileStream(sPath, System.IO.FileMode.OpenOrCreate);

                        // Set the length to 250Kb
                        Byte[] bytes = new byte[fs.Length];
                        fs.Read(bytes, 0, (int)fs.Length);
                        fs.Close();
                        System.IO.FileStream fs2 = new System.IO.FileStream(sPath, System.IO.FileMode.Create);
                        fs2.Write(bytes, (int)bytes.Length - 0, 0);
                        fs2.Flush();
                        Estatus += "Truncado 4 correcto ReC.Log" + "(" + filelength.ToString() + ")";
                    }
                    catch (Exception Ex)
                    {
                        Estatus += "Truncado 4 error ReC.Log" + "(" + filelength.ToString() + ")";
                    }
                    
                }
                catch (Exception Ex)
                {
                    Estatus += "Error al truncar el archivo ReC.Log " + Ex.Message + Estatus;
                }
            }
        }
    }
}