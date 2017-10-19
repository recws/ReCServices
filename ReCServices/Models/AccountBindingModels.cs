using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ReCServices.Models
{
    // Modelos usados como parámetros para las acciones de AccountController.

    public class AddExternalLoginBindingModel
    {
        [Required]
        [Display(Name = "Token de acceso externo")]
        public string ExternalAccessToken { get; set; }
    }

    public class ChangePasswordBindingModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña actual")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar la nueva contraseña")]
        [Compare("NewPassword", ErrorMessage = "La nueva contraseña y la contraseña de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterBindingModel
    {

        [Display(Name = "IdUsuario")]
        public Nullable<int> IdUsuario { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "El {0} debe tener al menos {2} caracteres.", MinimumLength = 5)]
        [Display(Name = "Usuario")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y la contraseña de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "El {0} debe tener al menos {2} caracteres.", MinimumLength = 5)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "Empresa")]
        public string Empresa { get; set; }

        [Display(Name = "Foto")]
        public string Foto { get; set; }

        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }


        [Display(Name = "Grupos")]
        public string Grupos { get; set; }


        [Display(Name = "Operaciones")]
        public string Operaciones { get; set; }

        [Display(Name = "Permitir Acceso Simultaneo?")]
        public bool AccesoSimultaneo { get; set; }
        [Display(Name = "Permitir Repartir Carga?")]
        public bool PermitirRepartirCarga { get; set; }
        [Display(Name = "Permitir Logeo Externo?")]
        public bool PermitirLogeoExterno { get; set; }
        [StringLength(30)]
        [Display(Name = "Telefono")]
        public string Telefono { get; set; }
        [StringLength(30)]
        [Display(Name = "Radio")]
        public string Radio { get; set; }
        [Display(Name = "Nextel")]
        public string Nextel { get; set; }
        [Display(Name = "Skype")]
        public string Skype { get; set; }
        [Display(Name = "Activo")]
        public bool Activo { get; set; }
        [Display(Name = "Fecha Alta")]
        public Nullable<DateTime> FechaAltaUTC { get; set; }
        [Display(Name = "Fecha Acceso")]
        public Nullable<DateTime> FechaAccesoUTC { get; set; }
        [Display(Name = "IdTimeZone")]
        public Nullable<int> IdTimeZone { get; set; }
        [Display(Name = "Filtro Viajes Monitoreo")]
        public string FiltroViajesMonitoreo { get; set; }
        [Display(Name = "Enterado Ultimos Cambios")]
        public bool EnteradoUltimosCambios { get; set; }
        [Display(Name = "Ver Demo")]
        public bool VerDemo { get; set; }

        //[Required]
        //[Display(Name = "Correo electrónico")]
        //public string Email { get; set; }

        //[Required]
        //[StringLength(100, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 6)]
        //[DataType(DataType.Password)]
        //[Display(Name = "Contraseña")]
        //public string Password { get; set; }

        //[DataType(DataType.Password)]
        //[Display(Name = "Confirmar contraseña")]
        //[Compare("Password", ErrorMessage = "La contraseña y la contraseña de confirmación no coinciden.")]
        //public string ConfirmPassword { get; set; }
    }

    public class RegisterExternalBindingModel
    {
        [Required]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }
    }

    public class RemoveLoginBindingModel
    {
        [Required]
        [Display(Name = "Proveedor de inicio de sesión")]
        public string LoginProvider { get; set; }

        [Required]
        [Display(Name = "Clave de proveedor")]
        public string ProviderKey { get; set; }
    }

    public class SetPasswordBindingModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar la nueva contraseña")]
        [Compare("NewPassword", ErrorMessage = "La nueva contraseña y la contraseña de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }
    }
}
