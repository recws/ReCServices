using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Configuration;
using Microsoft.AspNet.SignalR;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Net.Mail;

namespace ReCServices
{
    public class MonitoreoAutomatico
    {
        //Monitoreo db = new Monitoreo();

        //public void MonitoreoAutomatico_SiguienteViaje() {
            
        //    var context = GlobalHost.ConnectionManager.GetHubContext<GPSHub>();
        //    Monitoreada monitoreada = new Monitoreada();
        //    monitoreada.IdUsuario = 0;

        //    List<MonitoreoAutomatico_CargaSiguienteViaje_Result> MonitoreoAutomatico_CargaSiguienteViaje;

        //    MonitoreoAutomatico_CargaSiguienteViaje = db.MonitoreoAutomatico_CargaSiguienteViaje(1).ToList();
        //    if (MonitoreoAutomatico_CargaSiguienteViaje.Count() == 1)
        //    {
        //        //Consulta la ultima posicion
        //        WS_CONTEXT db = new WS_CONTEXT("WS_CONTEXT_PROD");
        //        if (MonitoreoAutomatico_CargaSiguienteViaje[0].Plataforma=="ReC")
        //        {                    
        //            var UltimaPosicion = db.WS_GPS_ConsultaUltimaPosicion(MonitoreoAutomatico_CargaSiguienteViaje[0].IMEI).ToList();
        //        }
        //        //Revisa si hay retraso de carga
        //        //if (MonitoreoAutomatico_CargaSiguienteViaje[0])
        //        //{

        //        //}
        //        //Revisa si hay retraso de salida
        //        //Revisa si hay retraso de cita
        //        //Revisa si hay entrada de geocerca


        //        //context.Clients.All.broadCastMessage("Server", "MonitoreoAutomatico", MonitoreoAutomatico_CargaSiguienteViaje[0]);

        //        //Revisa si hay salida de geocerca
        //        //Revisa si hay desvio de ruta
        //        //Revisa si hay Alguna alerta critica (apertura de puertas, panico, nogps, jammer, desconexion, bateria baja, etc)
        //        //Revisa si hay paradas no autorizadas (incluir, medir el tiempo detenido y con ignicion, sin avanzar km (medir tiempo total)

        //        //Al final envia mensaje de que fue monitoreada por el sistema.
        //        //Ejecuta guardado de monitoreada por el sistema

        //        //monitoreada.IdViaje = MonitoreoAutomatico_CargaSiguienteViaje[0].IdViaje;
        //        //monitoreada.IdEstatusViaje = 0;
        //        //monitoreada.FechaUltimoMonitoreo = DateTime.UtcNow;
        //        //context.Clients.All.broadCastMessage("Server", "MonitoreoAutomatico", monitoreada);
        //    }

        //}


        //private class Monitoreada
        //{
        //    public int IdViaje { get; set; }
        //    public int IdEstatusViaje { get; set; }
        //    //public DateTime FechaHoraCargaReal { get; set; }
        //    //public DateTime FechaHoraSalidaReal { get; set; }
        //    public DateTime FechaUltimoMonitoreo { get; set; }
        //    public int IdUsuario { get; set; }
        //    public string Propiedades { get; set; }
        //}
    }
}