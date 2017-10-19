using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ReCServices
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "IWebService" en el código y en el archivo de configuración a la vez.
    [ServiceContract]
    public interface IWebService
    {

        ///
        /// <summary>Look up stock ticker and return the company name.</summary>
        /// <param name="ticker">The stock ticker to lookup</param>
        /// <returns>The name of the stock</returns>
        [OperationContract]
        RespuestaServicio InsertaSimple(EventoSimple eventosimple);
        //[OperationContract]
        //RespuestaServicio InsertaMultiple(EventoSimple eventosimple);
    }

    ///
    /// <summary>Inserta un evento del dispositivo.</summary>
    ///
    [DataContract]
    public class EventoSimple
    {
        [DataMember]
        public String Usuario { get; set; }
        [DataMember]
        public String Password { get; set; }
        //[DataMember]
        //public int IdGrupo { get; set; }
        //[DataMember]
        //public int IdOperacion { get; set; }
        //[DataMember]
        //public String Eco { get; set; }
        [DataMember]
        public String IMEI { get; set; }
        [DataMember]
        public string CodigoEvento { get; set; }
        [DataMember]
        public decimal Lat { get; set; }
        [DataMember]
        public decimal Lng { get; set; }
        [DataMember]
        public string Ubicacion { get; set; }
        [DataMember]
        public Boolean GPSValido { get; set; }
        [DataMember]
        public int Velocidad { get; set; }
        [DataMember]
        public int Direccion { get; set; }
        [DataMember]
        public int NivelBateria { get; set; }
        [DataMember]
        public int KMOdometro { get; set; }
        [DataMember]
        public DateTime FechaHoraGeneracion { get; set; }
        [DataMember]
        public DateTime FechaHoraRecepcion { get; set; }
    }

    public class RespuestaServicio
    {
        public int Indicador { get; set; }
        public string Mensaje { get; set; }

    }

    public class Result
    {
        public int Indicador { get; set; }
        public string Mensaje { get; set; }
        public int IdRegistroAfectado { get; set; }
    }
}
