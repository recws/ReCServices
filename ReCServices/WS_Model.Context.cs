﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ReCServices
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class WS_CONTEXT : DbContext
    {
        public WS_CONTEXT()
            : base("name=WS_CONTEXT")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
    
        public virtual ObjectResult<WS_GPS_ConsultaHistoricoUTC_Result> WS_GPS_ConsultaHistoricoUTC(string iMEI, Nullable<System.DateTime> fechaInicioUTC, Nullable<System.DateTime> fechaFinUTC)
        {
            var iMEIParameter = iMEI != null ?
                new ObjectParameter("IMEI", iMEI) :
                new ObjectParameter("IMEI", typeof(string));
    
            var fechaInicioUTCParameter = fechaInicioUTC.HasValue ?
                new ObjectParameter("FechaInicioUTC", fechaInicioUTC) :
                new ObjectParameter("FechaInicioUTC", typeof(System.DateTime));
    
            var fechaFinUTCParameter = fechaFinUTC.HasValue ?
                new ObjectParameter("FechaFinUTC", fechaFinUTC) :
                new ObjectParameter("FechaFinUTC", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<WS_GPS_ConsultaHistoricoUTC_Result>("WS_GPS_ConsultaHistoricoUTC", iMEIParameter, fechaInicioUTCParameter, fechaFinUTCParameter);
        }
    
        public virtual ObjectResult<WS_GPS_ConsultaIncidenciasAutomaticas_Result> WS_GPS_ConsultaIncidenciasAutomaticas(string usuario)
        {
            var usuarioParameter = usuario != null ?
                new ObjectParameter("Usuario", usuario) :
                new ObjectParameter("Usuario", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<WS_GPS_ConsultaIncidenciasAutomaticas_Result>("WS_GPS_ConsultaIncidenciasAutomaticas", usuarioParameter);
        }
    
        public virtual ObjectResult<WS_GPS_ConsultaUltimaPosicion_Result> WS_GPS_ConsultaUltimaPosicion(string iMEI)
        {
            var iMEIParameter = iMEI != null ?
                new ObjectParameter("IMEI", iMEI) :
                new ObjectParameter("IMEI", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<WS_GPS_ConsultaUltimaPosicion_Result>("WS_GPS_ConsultaUltimaPosicion", iMEIParameter);
        }
    
        public virtual ObjectResult<WS_GPS_ConsultaxUsuarioUTC_Result> WS_GPS_ConsultaxUsuarioUTC(string usuario, Nullable<System.DateTime> fechaInicioUTC, Nullable<System.DateTime> fechaFinUTC)
        {
            var usuarioParameter = usuario != null ?
                new ObjectParameter("Usuario", usuario) :
                new ObjectParameter("Usuario", typeof(string));
    
            var fechaInicioUTCParameter = fechaInicioUTC.HasValue ?
                new ObjectParameter("FechaInicioUTC", fechaInicioUTC) :
                new ObjectParameter("FechaInicioUTC", typeof(System.DateTime));
    
            var fechaFinUTCParameter = fechaFinUTC.HasValue ?
                new ObjectParameter("FechaFinUTC", fechaFinUTC) :
                new ObjectParameter("FechaFinUTC", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<WS_GPS_ConsultaxUsuarioUTC_Result>("WS_GPS_ConsultaxUsuarioUTC", usuarioParameter, fechaInicioUTCParameter, fechaFinUTCParameter);
        }
    
        public virtual ObjectResult<WS_GPS_InsertaSimple_Result> WS_GPS_InsertaSimple(string usuario, string iMEI, string codigoEvento, Nullable<decimal> lat, Nullable<decimal> lng, string ubicacion, Nullable<bool> gPSValido, Nullable<int> velocidad, Nullable<int> direccion, Nullable<int> nivelBateria, Nullable<int> kMOdometro, Nullable<System.DateTime> fechaHoraGeneracion, Nullable<System.DateTime> fechaHoraRecepcion)
        {
            var usuarioParameter = usuario != null ?
                new ObjectParameter("Usuario", usuario) :
                new ObjectParameter("Usuario", typeof(string));
    
            var iMEIParameter = iMEI != null ?
                new ObjectParameter("IMEI", iMEI) :
                new ObjectParameter("IMEI", typeof(string));
    
            var codigoEventoParameter = codigoEvento != null ?
                new ObjectParameter("CodigoEvento", codigoEvento) :
                new ObjectParameter("CodigoEvento", typeof(string));
    
            var latParameter = lat.HasValue ?
                new ObjectParameter("Lat", lat) :
                new ObjectParameter("Lat", typeof(decimal));
    
            var lngParameter = lng.HasValue ?
                new ObjectParameter("Lng", lng) :
                new ObjectParameter("Lng", typeof(decimal));
    
            var ubicacionParameter = ubicacion != null ?
                new ObjectParameter("Ubicacion", ubicacion) :
                new ObjectParameter("Ubicacion", typeof(string));
    
            var gPSValidoParameter = gPSValido.HasValue ?
                new ObjectParameter("GPSValido", gPSValido) :
                new ObjectParameter("GPSValido", typeof(bool));
    
            var velocidadParameter = velocidad.HasValue ?
                new ObjectParameter("Velocidad", velocidad) :
                new ObjectParameter("Velocidad", typeof(int));
    
            var direccionParameter = direccion.HasValue ?
                new ObjectParameter("Direccion", direccion) :
                new ObjectParameter("Direccion", typeof(int));
    
            var nivelBateriaParameter = nivelBateria.HasValue ?
                new ObjectParameter("NivelBateria", nivelBateria) :
                new ObjectParameter("NivelBateria", typeof(int));
    
            var kMOdometroParameter = kMOdometro.HasValue ?
                new ObjectParameter("KMOdometro", kMOdometro) :
                new ObjectParameter("KMOdometro", typeof(int));
    
            var fechaHoraGeneracionParameter = fechaHoraGeneracion.HasValue ?
                new ObjectParameter("FechaHoraGeneracion", fechaHoraGeneracion) :
                new ObjectParameter("FechaHoraGeneracion", typeof(System.DateTime));
    
            var fechaHoraRecepcionParameter = fechaHoraRecepcion.HasValue ?
                new ObjectParameter("FechaHoraRecepcion", fechaHoraRecepcion) :
                new ObjectParameter("FechaHoraRecepcion", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<WS_GPS_InsertaSimple_Result>("WS_GPS_InsertaSimple", usuarioParameter, iMEIParameter, codigoEventoParameter, latParameter, lngParameter, ubicacionParameter, gPSValidoParameter, velocidadParameter, direccionParameter, nivelBateriaParameter, kMOdometroParameter, fechaHoraGeneracionParameter, fechaHoraRecepcionParameter);
        }
    
        public virtual ObjectResult<WS_GPS_ListaGPSxProveedor_Result> WS_GPS_ListaGPSxProveedor(Nullable<int> idTransportista, string proveedor)
        {
            var idTransportistaParameter = idTransportista.HasValue ?
                new ObjectParameter("IdTransportista", idTransportista) :
                new ObjectParameter("IdTransportista", typeof(int));
    
            var proveedorParameter = proveedor != null ?
                new ObjectParameter("Proveedor", proveedor) :
                new ObjectParameter("Proveedor", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<WS_GPS_ListaGPSxProveedor_Result>("WS_GPS_ListaGPSxProveedor", idTransportistaParameter, proveedorParameter);
        }
    
        public virtual int WS_GPS_SincronizaGPS()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("WS_GPS_SincronizaGPS");
        }
    
        public virtual int WS_GPS_SincronizaUsuario()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("WS_GPS_SincronizaUsuario");
        }
    
        public virtual ObjectResult<WS_GPS_ValidaUsuario_Result> WS_GPS_ValidaUsuario(string usuario, string iMEI)
        {
            var usuarioParameter = usuario != null ?
                new ObjectParameter("Usuario", usuario) :
                new ObjectParameter("Usuario", typeof(string));
    
            var iMEIParameter = iMEI != null ?
                new ObjectParameter("IMEI", iMEI) :
                new ObjectParameter("IMEI", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<WS_GPS_ValidaUsuario_Result>("WS_GPS_ValidaUsuario", usuarioParameter, iMEIParameter);
        }
    
        public virtual ObjectResult<WS_GPS_ValidaUsuarioyPassword_Result> WS_GPS_ValidaUsuarioyPassword(string usuario, string password, string iMEI)
        {
            var usuarioParameter = usuario != null ?
                new ObjectParameter("Usuario", usuario) :
                new ObjectParameter("Usuario", typeof(string));
    
            var passwordParameter = password != null ?
                new ObjectParameter("Password", password) :
                new ObjectParameter("Password", typeof(string));
    
            var iMEIParameter = iMEI != null ?
                new ObjectParameter("IMEI", iMEI) :
                new ObjectParameter("IMEI", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<WS_GPS_ValidaUsuarioyPassword_Result>("WS_GPS_ValidaUsuarioyPassword", usuarioParameter, passwordParameter, iMEIParameter);
        }
    }
}
