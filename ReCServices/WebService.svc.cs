using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ReCServices
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "WebService" en el código, en svc y en el archivo de configuración a la vez.
    // NOTA: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione WebService.svc o WebService.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class WebService : IWebService
    {
        public RespuestaServicio InsertaSimple(EventoSimple eventosimple)
        {
            Negocio negocio = new Negocio();
            return negocio.InsertaSimple(eventosimple, "WS");
        }

        //public RespuestaServicio InsertaMultiple(EventoSimple eventosimple)
        //{
        //    ReCEntities db = new ReCEntities();
        //    RespuestaServicio respuesta = new RespuestaServicio();

        //    respuesta.Indicador = 1;
        //    respuesta.Mensaje = "OK";
        //    return respuesta;
        //}
    }
}
