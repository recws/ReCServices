using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Quartz;
using Quartz.Impl;
using Quartz.Core;
using System.Web;


[assembly: OwinStartup(typeof(ReCServices.Startup))]

namespace ReCServices
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            ConfigureAuth(app);

            var config = new HubConfiguration();
            config.EnableJSONP = true;
            config.EnableDetailedErrors = true;

            app.MapSignalR();
            JobScheduler.Start();


            //// Branch the pipeline here for requests that start with "/signalr"
            //app.Map("/signalr", map =>
            //{
            //    // Setup the CORS middleware to run before SignalR.
            //    // By default this will allow all origins. You can 
            //    // configure the set of origins and/or http verbs by
            //    // providing a cors options with a different policy.
            //    map.UseCors(CorsOptions.AllowAll);
            //    var hubConfiguration = new HubConfiguration
            //    {
            //        // You can enable JSONP by uncommenting line below.
            //        // JSONP requests are insecure but some older browsers (and some
            //        // versions of IE) require JSONP to work cross domain
            //        // EnableJSONP = true
            //    };
            //    // Run the SignalR pipeline. We're not using MapSignalR
            //    // since this branch already runs under the "/signalr"
            //    // path.
            //    map.RunSignalR(hubConfiguration);
            //});
        }
 
    }


    public class JobScheduler
    {
        public static void Start()
        {
            ///////////////////  SCHEDULER  //////////////////////////////////
            IScheduler scheduler_Pruebas = StdSchedulerFactory.GetDefaultScheduler();
            scheduler_Pruebas.Start();
            IJobDetail job_Pruebas = JobBuilder.Create<Pruebas>().Build();

            ITrigger trigger_Pruebas = TriggerBuilder.Create()
            .WithIdentity("Pruebas", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(1)
            .RepeatForever())
            .Build();

            scheduler_Pruebas.ScheduleJob(job_Pruebas, trigger_Pruebas);

            ///////////////////  SCHEDULER 1 MINUTO  //////////////////////////////////
            IScheduler scheduler_SincronizaCada1min = StdSchedulerFactory.GetDefaultScheduler();
            scheduler_SincronizaCada1min.Start();
            IJobDetail job_SincronizaCada1min = JobBuilder.Create<SincronizaCada1min>().Build();

            ITrigger trigger_SincronizaCada1min = TriggerBuilder.Create()
            .WithIdentity("trigger_SincronizaCada1min", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(1)
            .RepeatForever())
            .Build();

            scheduler_SincronizaCada1min.ScheduleJob(job_SincronizaCada1min, trigger_SincronizaCada1min);

            ///////////////////  SCHEDULER 2 MINUTOS  //////////////////////////////////
            IScheduler scheduler_SincronizaCada2min = StdSchedulerFactory.GetDefaultScheduler();
            scheduler_SincronizaCada2min.Start();
            IJobDetail job_SincronizaCada2min = JobBuilder.Create<SincronizaCada2min>().Build();

            ITrigger trigger_SincronizaCada2min = TriggerBuilder.Create()
            .WithIdentity("trigger_SincronizaCada2min", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(2)
            .RepeatForever())
            .Build();

            scheduler_SincronizaCada2min.ScheduleJob(job_SincronizaCada2min, trigger_SincronizaCada2min);

            ///////////////////  SCHEDULER  //////////////////////////////////
            IScheduler scheduler_SincronizaCada3min = StdSchedulerFactory.GetDefaultScheduler();
            scheduler_SincronizaCada3min.Start();
            IJobDetail job_SincronizaCada3min = JobBuilder.Create<SincronizaCada3min>().Build();

            ITrigger trigger_SincronizaCada3min = TriggerBuilder.Create()
            .WithIdentity("trigger_SincronizaCada3min", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(3)
            .RepeatForever())
            .Build();

            scheduler_SincronizaCada3min.ScheduleJob(job_SincronizaCada3min, trigger_SincronizaCada3min);

            ///////////////////  SCHEDULER  //////////////////////////////////
            IScheduler scheduler_SincronizaCada4min = StdSchedulerFactory.GetDefaultScheduler();
            scheduler_SincronizaCada4min.Start();
            IJobDetail job_SincronizaCada4min = JobBuilder.Create<SincronizaCada4min>().Build();

            ITrigger trigger_SincronizaCada4min = TriggerBuilder.Create()
            .WithIdentity("trigger_SincronizaCada4min", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(4)
            .RepeatForever())
            .Build();

            scheduler_SincronizaCada4min.ScheduleJob(job_SincronizaCada4min, trigger_SincronizaCada4min);

            ///////////////////  SCHEDULER  //////////////////////////////////
            IScheduler scheduler_SincronizaCada5min = StdSchedulerFactory.GetDefaultScheduler();
            scheduler_SincronizaCada5min.Start();
            IJobDetail job_SincronizaCada5min = JobBuilder.Create<SincronizaCada5min>().Build();

            ITrigger trigger_SincronizaCada5min = TriggerBuilder.Create()
            .WithIdentity("trigger_SincronizaCada5min", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(5)
            .RepeatForever())
            .Build();

            scheduler_SincronizaCada5min.ScheduleJob(job_SincronizaCada5min, trigger_SincronizaCada5min);
            
            ///////////////////  SCHEDULER  //////////////////////////////////
            IScheduler scheduler_SincronizaCada10min = StdSchedulerFactory.GetDefaultScheduler();
            scheduler_SincronizaCada10min.Start();
            IJobDetail job_SincronizaCada10min = JobBuilder.Create<SincronizaCada10min>().Build();

            ITrigger trigger_SincronizaCada10min = TriggerBuilder.Create()
            .WithIdentity("trigger_SincronizaCada10min", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(10)
            .RepeatForever())
            .Build();

            scheduler_SincronizaCada10min.ScheduleJob(job_SincronizaCada10min, trigger_SincronizaCada10min);

            ///////////////////  SCHEDULER  //////////////////////////////////
            IScheduler scheduler_SincronizaCada30seg = StdSchedulerFactory.GetDefaultScheduler();
            scheduler_SincronizaCada30seg.Start();
            IJobDetail job_SincronizaCada30seg = JobBuilder.Create<SincronizaCada30seg>().Build();

            ITrigger trigger_SincronizaCada30seg = TriggerBuilder.Create()
            .WithIdentity("trigger_SincronizaCada30seg", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
            .WithIntervalInSeconds(30)
            .RepeatForever())
            .Build();

            scheduler_SincronizaCada30seg.ScheduleJob(job_SincronizaCada30seg, trigger_SincronizaCada30seg);

            ///////////////////  SCHEDULER  //////////////////////////////////
            IScheduler scheduler_LogLlenoCadahora = StdSchedulerFactory.GetDefaultScheduler();
            scheduler_LogLlenoCadahora.Start();
            IJobDetail job_LogLlenoCadahora = JobBuilder.Create<LogLlenoCadahora>().Build();

            ITrigger trigger_LogLlenoCadahora = TriggerBuilder.Create()
            .WithIdentity("trigger_LogLlenoCadahora", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(60)
            .RepeatForever())
            .Build();

            scheduler_LogLlenoCadahora.ScheduleJob(job_LogLlenoCadahora, trigger_LogLlenoCadahora);
            

        }
    }


    public class Pruebas : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            //Se pasaron a soluciones globlaes como aley (ver excel)
            //Apis.Wialon.WIALON_ObtenerPosicion("WS_Leyva", "huawei", "huawei2016", "b16a120f3fdab34dade89383ee0712cb79ECECE76F8ECAE55344826FC75993C214FAD9F4");  //Transportes LEYVA  provedor WS_AVLLogic
            //Apis.GrupoUDA.GrupoUDA_ObtenerPosicion("WS_SID", "WBS_PAN-SUVI", "123456");  //Cambiaron de plataforma... Ya no se va a contemplar este proveedor
            //Apis.Soltrack.Soltrack_ObtenerPosicion("WS_THernandez", "", "");  //FALTA QUE NOS DEN EL LISTADO DE IMEI  Ya cancelo con este proveedor


            //Leyva SG (Soluciones Globales)
            //USER Y PWDde cuenta espejo no funcionan en WS 
            //Apis.SolucionesGlobales.SolucionesGloblaes_ObtenerPosicion("WS_TMP", "Huawei", "huawei2017");




            //Apis.PositionGate.PositionGate_ObtenerPosicion("WS_TMP", "", "78590AAF421B58B347492656C11340ED");

            //Modificar para transportes ROMO
            //Apis.MonitoreoYRastereo.MonitoreoYRastreo_ObtenerPosicion("WS_Diez", "webservice", "Gtslo98_213#", "DIEZ");




            //verificando            
            //Apis.Cybermapa.Cybermapa_ObtenerPosicion("WS_RBA", "PANA", "MUNDIAL2016");  //Transportes RBA






            //Apis.OmnitracsCC.Omnitracs_ObtenerPosicion("WS_Elola", "mx80566", "29205mx80566", "123456");



        }
    }

    public class SincronizaCada30seg : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
#if !DEBUG
            //Negocio metodos = new Negocio();
            //metodos.NotificacionIncidencia();            
#endif
        }
    }

    public class SincronizaCada1min : IJob
    {
        public void Execute(IJobExecutionContext context)
        {

#if !DEBUG
            Apis.LaNegrita lanegrita = new Apis.LaNegrita();
            lanegrita.WebServiceInsertaLANEGRITA();
            Apis.Seinext.Seinext_ObtenerPosicion("WS_Avensica", "AVENSICA", "BavensicaA");  //Seinext -> Avensica
#endif
        }
    }
    public class SincronizaCada2min : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
#if !DEBUG            
            //PRUEBA 3
            Apis.Boson.BOSON_ObtenerPosicion("WS_NewPick", "olopez@recsolutions.tech", "R2e0C1C8c");  //COOPERTIRES  //Transportes USAMEX CARRIER  //No lleva token
            Apis.Omnitracs.Omnitracs_ObtenerPosicion("WS_Masetto", "", ""); //COOPERTIRES  (el usuario y contraseña lo lleva en web.config, en headers del endpoint)
            Apis.OmnitracsCC.Omnitracs_ObtenerPosicion("WS_Elola", "mx80566", "29205mx80566", "123456");
            Apis.OmnitracsCC.Omnitracs_ObtenerPosicion("WS_UsaMex", "mxfa811", "29244mxfa811", "Abcd1234");
            Apis.Unicomm.Unicomm_ObtenerPosicion("WS_SagCargo", "token=7a09096bf2a1dec49d2e3cb9b817ab4c", "", "");
#endif
        }
    }

    public class SincronizaCada3min : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
#if !DEBUG

            Apis.Moving.Moving_ObtenerPosicion("WS_Moving", "api_moving", "k0VU8XQ5");  // HNOS HERNANDEZ MOVING - COOPER
            Apis.Wialon.WIALON_ObtenerPosicion("WS_Guarneros", "Tguarneros", "L0cal1zar", "d631f588de587f4f7f3e21275151a90fE24BDB7F1B7EAB281936C06D139B56B1826750A8");  //Transportes Guarneros
#endif
        }
    }

    public class SincronizaCada4min : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
#if !DEBUG
                        
            Apis.Boson.BOSON_ObtenerPosicion("WS_TIslas", "trans.log.is@hotmail.com", "Es29Htj8M5");  //Transportes T. ISLAS   Proveedor: KOSMOS
            Apis.SGIT.SGIT_ObtenerPosicion("WS_TIslas", "antonio", "antonio69");  //Transportes T. ISLAS  Proveedor: SGIT
            Apis.Boson.BOSON_ObtenerPosicion("WS_TLIT", "a.rivera@interestatalit.com", "panalpinalit");  //Transportes T. LIT  //No lleva token
            APIS.REESER_ObtenerPosicion("WS_IMTAutoT", "FELIPE-AGUILAR", "U2FsdGVkX1+GHY1Z/SObeDjZCZ4ARuSUN80EtrPOn54=");
            
            Apis.Quiala.QUIALA_ObtenerPosicion("WS_Quiala", "ws-quiala", "Qui2kdh*bn6");
            Apis.Vectro.VECTRO_ObtenerPosicion("WS_TPina", "", "");
            Apis.Troncalnet.Troncalnet_ObtenerPosicion("WS_TOrtiz", "", ""); //COOPERTIRES 
            
            Apis.Boson.BOSON_ObtenerPosicion("WS_UsaMex", "monitoreo@usamexcarrier.com", "UsaMexCa2017");  //COOPERTIRES  //Transportes USAMEX CARRIER  //No lleva token
            
            Apis.GpsMonitor.GGpsMonitor_ObtenerPosicion("WS_Trapol"); //Trapol - GPS Monitor
            Apis.Cybermapa.Cybermapa_ObtenerPosicion("WS_RBA", "PANA", "MUNDIAL2016");  //Transportes RBA

            Apis.MonitoreoYRastereo.MonitoreoYRastreo_ObtenerPosicion("WS_Diez", "webservice", "Gtslo98_213#", "DIEZ");
            Apis.Sama.Sama_ObtenerPosicion("WS_Trapol", "panalpina-trapol", "Andromeda500");  //TRAPOL  ProveedorGPS: SAMA
            Apis.PlataformaLVT.PlataformaLVT_ObtenerPosicion("WS_8W", "8W_USUARIO_API", "API_EXPRESS_1", "1868c5a2-ac8e-4486-bae1-3b10e3645bef", "8W"); //Cooper -8w
#endif
        }
    }

    public class SincronizaCada5min : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
#if !DEBUG
            Apis.GrupoGCP.GrupoGCP_ObtenerPosicion("WS_GrupoGCP", "", ""); //La contraseña va dentro ya.
            Apis.RedGPS.RedGPS_ObtenerPosicion("WS_TIbarra", "panalpina", "transportesibarra");  //Transportes T. IBARRA 
            Apis.Astus.ObtenereInsertar("WS_SEND", "webservices1@send.com", "1qaz2345");                        
            Apis.Cybermapa.Cybermapa_ObtenerPosicion("WS_TRuiz", "logisticos", "monica321");  //Transportes RUIZ
            Apis.Boson.BOSON_ObtenerPosicion("WS_Transcar", "panalpina@transcar.com.mx", "Panalpin2017");  //Transportes TRANSCAR  //No lleva token
            Apis.OmnitracsCC.Omnitracs_ObtenerPosicion("WS_Jaguar", "mx4b4b3", "29162mx4b4b3", "123456");
            Apis.Cybermapa.Cybermapa_ObtenerPosicion("WS_Trameriv", "trameriv", "mexico321");  //Transportes TRAMERIV
            Apis.Boson.BOSON_ObtenerPosicion("WS_Gomez", "monitoreo@recsolutions.tech", "TraNsp0rGMZ2");  //Transportes GOMEZ  //No lleva token
            Apis.ZeekGPS.ZeekGPS_ObtenerPosicion("WS_Panamericano", "Panalpina", "cmGxtFia", "iPinssoMperPogm");  //Transportes PANAMERICANO                         
            
            Apis.GpsTotal.GpsTotal_ObtenerPosicion("WS_ETI", "0929ccdd-cb9a-4d47-8c1f-22850a0b71d9");  // FALTA LA PARTE DE ENCABEZADO PARA CONSULTAR POR IMEI
            Apis.TrackJack.Trackjack_ObtenerPosicion("WS_CargoServices", "COOPREC", "*070820/"); //Trackjack
            Apis.Rosello.Rosello_ObtenerPosicion("WS_KLAS", "", "f6450ha45c00b1249bbe35"); //Klas
            
#endif
        }
    }
    
    public class SincronizaCada10min : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
#if !DEBUG
            //Apis.GrupoUDA.GrupoUDA_ObtenerPosicion("WS_SID", "WBS_PAN-SUVI", "123456");

#endif
        }
    }

    public class LogLlenoCadahora : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
#if !DEBUG

            Negocio metodos = new Negocio();
            metodos.WS_GPS_SincronizaUsuario();
            metodos.WS_GPS_SincronizaGPS();

            //Negocio metodos = new Negocio();
            metodos.ReportexCorreoLogLleno();
            //metodos.TruncarReCLog();
#endif
        }
    }
}
