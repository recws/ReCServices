using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Web;

namespace ReCServices.Models
{
    // Para agregar datos de perfil al usuario, agregue más propiedades a la clase ApplicationUser. Para obtener más información, visite http://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {
        public Nullable<int> IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Empresa { get; set; }
        public string Foto { get; set; }
        public string Grupos { get; set; }
        public string Operaciones { get; set; }
        public bool AccesoSimultaneo { get; set; }
        public bool PermitirRepartirCarga { get; set; }
        public bool PermitirLogeoExterno { get; set; }
        public string Telefono { get; set; }
        public string Radio { get; set; }
        public string Nextel { get; set; }
        public string Skype { get; set; }
        public bool Activo { get; set; }
        public Nullable<DateTime> FechaAltaUTC { get; set; }
        public Nullable<DateTime> FechaAccesoUTC { get; set; }
        public Nullable<int> IdTimeZone { get; set; }
        public string FiltroViajesMonitoreo { get; set; }
        public bool EnteradoUltimosCambios { get; set; }
        public bool VerDemo { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Tenga en cuenta que el valor de authenticationType debe coincidir con el definido en CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Agregar aquí notificaciones personalizadas de usuario
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(string ConexionDinamica)
            : base(ConexionDinamica, throwIfV1Schema: false)
        {
        }
        
        public static ApplicationDbContext Create()
        {
            string DomainName = "";
            try
            {
                DomainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            }
            catch (Exception Ex)
            {
                DomainName = "";
            }

            string ConexionDinamica = "";

            #if DEBUG
            ConexionDinamica = "WS_TEST";
            #else
                        if (DomainName == "http://ws.recsolutions.tech")
                        {
                            ConexionDinamica = "WS_PROD";
                        }
                        else
                        {
                            ConexionDinamica = "WS_TEST";
                        }
            #endif
            return new ApplicationDbContext(ConexionDinamica);
        }
    }
}