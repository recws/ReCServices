﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ReCServices.WS_Trackjack_Service {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TrackJackData", Namespace="http://www.trackjack.mx/")]
    [System.SerializableAttribute()]
    public partial class TrackJackData : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string GpsIDField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string UnitIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LatitudeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LongitudeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string AltitudeField;
        
        private double OdometerField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string SpeedGpsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CourseField;
        
        private int NumSateField;
        
        private bool IgnitionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DateGpsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string UnitPlateField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string IdGpsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string EventDescriptionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string EventoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string AzimuthField;
        
        private double direccionField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public string GpsID {
            get {
                return this.GpsIDField;
            }
            set {
                if ((object.ReferenceEquals(this.GpsIDField, value) != true)) {
                    this.GpsIDField = value;
                    this.RaisePropertyChanged("GpsID");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public string UnitId {
            get {
                return this.UnitIdField;
            }
            set {
                if ((object.ReferenceEquals(this.UnitIdField, value) != true)) {
                    this.UnitIdField = value;
                    this.RaisePropertyChanged("UnitId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string Latitude {
            get {
                return this.LatitudeField;
            }
            set {
                if ((object.ReferenceEquals(this.LatitudeField, value) != true)) {
                    this.LatitudeField = value;
                    this.RaisePropertyChanged("Latitude");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public string Longitude {
            get {
                return this.LongitudeField;
            }
            set {
                if ((object.ReferenceEquals(this.LongitudeField, value) != true)) {
                    this.LongitudeField = value;
                    this.RaisePropertyChanged("Longitude");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string Altitude {
            get {
                return this.AltitudeField;
            }
            set {
                if ((object.ReferenceEquals(this.AltitudeField, value) != true)) {
                    this.AltitudeField = value;
                    this.RaisePropertyChanged("Altitude");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=5)]
        public double Odometer {
            get {
                return this.OdometerField;
            }
            set {
                if ((this.OdometerField.Equals(value) != true)) {
                    this.OdometerField = value;
                    this.RaisePropertyChanged("Odometer");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=6)]
        public string SpeedGps {
            get {
                return this.SpeedGpsField;
            }
            set {
                if ((object.ReferenceEquals(this.SpeedGpsField, value) != true)) {
                    this.SpeedGpsField = value;
                    this.RaisePropertyChanged("SpeedGps");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=7)]
        public string Course {
            get {
                return this.CourseField;
            }
            set {
                if ((object.ReferenceEquals(this.CourseField, value) != true)) {
                    this.CourseField = value;
                    this.RaisePropertyChanged("Course");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=8)]
        public int NumSate {
            get {
                return this.NumSateField;
            }
            set {
                if ((this.NumSateField.Equals(value) != true)) {
                    this.NumSateField = value;
                    this.RaisePropertyChanged("NumSate");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=9)]
        public bool Ignition {
            get {
                return this.IgnitionField;
            }
            set {
                if ((this.IgnitionField.Equals(value) != true)) {
                    this.IgnitionField = value;
                    this.RaisePropertyChanged("Ignition");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=10)]
        public string DateGps {
            get {
                return this.DateGpsField;
            }
            set {
                if ((object.ReferenceEquals(this.DateGpsField, value) != true)) {
                    this.DateGpsField = value;
                    this.RaisePropertyChanged("DateGps");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=11)]
        public string UnitPlate {
            get {
                return this.UnitPlateField;
            }
            set {
                if ((object.ReferenceEquals(this.UnitPlateField, value) != true)) {
                    this.UnitPlateField = value;
                    this.RaisePropertyChanged("UnitPlate");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=12)]
        public string IdGps {
            get {
                return this.IdGpsField;
            }
            set {
                if ((object.ReferenceEquals(this.IdGpsField, value) != true)) {
                    this.IdGpsField = value;
                    this.RaisePropertyChanged("IdGps");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=13)]
        public string EventDescription {
            get {
                return this.EventDescriptionField;
            }
            set {
                if ((object.ReferenceEquals(this.EventDescriptionField, value) != true)) {
                    this.EventDescriptionField = value;
                    this.RaisePropertyChanged("EventDescription");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=14)]
        public string Evento {
            get {
                return this.EventoField;
            }
            set {
                if ((object.ReferenceEquals(this.EventoField, value) != true)) {
                    this.EventoField = value;
                    this.RaisePropertyChanged("Evento");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=15)]
        public string Azimuth {
            get {
                return this.AzimuthField;
            }
            set {
                if ((object.ReferenceEquals(this.AzimuthField, value) != true)) {
                    this.AzimuthField = value;
                    this.RaisePropertyChanged("Azimuth");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=16)]
        public double direccion {
            get {
                return this.direccionField;
            }
            set {
                if ((this.direccionField.Equals(value) != true)) {
                    this.direccionField = value;
                    this.RaisePropertyChanged("direccion");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://www.trackjack.mx/", ConfigurationName="WS_Trackjack_Service.ServiceSoap")]
    public interface ServiceSoap {
        
        // CODEGEN: Se está generando un contrato de mensaje, ya que el nombre de elemento usuario del espacio de nombres http://www.trackjack.mx/ no está marcado para aceptar valores nil.
        [System.ServiceModel.OperationContractAttribute(Action="http://www.trackjack.mx/PosicionesIndividual", ReplyAction="*")]
        ReCServices.WS_Trackjack_Service.PosicionesIndividualResponse PosicionesIndividual(ReCServices.WS_Trackjack_Service.PosicionesIndividualRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.trackjack.mx/PosicionesIndividual", ReplyAction="*")]
        System.Threading.Tasks.Task<ReCServices.WS_Trackjack_Service.PosicionesIndividualResponse> PosicionesIndividualAsync(ReCServices.WS_Trackjack_Service.PosicionesIndividualRequest request);
        
        // CODEGEN: Se está generando un contrato de mensaje, ya que el nombre de elemento usuario del espacio de nombres http://www.trackjack.mx/ no está marcado para aceptar valores nil.
        [System.ServiceModel.OperationContractAttribute(Action="http://www.trackjack.mx/PosicionesTodos", ReplyAction="*")]
        ReCServices.WS_Trackjack_Service.PosicionesTodosResponse PosicionesTodos(ReCServices.WS_Trackjack_Service.PosicionesTodosRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.trackjack.mx/PosicionesTodos", ReplyAction="*")]
        System.Threading.Tasks.Task<ReCServices.WS_Trackjack_Service.PosicionesTodosResponse> PosicionesTodosAsync(ReCServices.WS_Trackjack_Service.PosicionesTodosRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class PosicionesIndividualRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="PosicionesIndividual", Namespace="http://www.trackjack.mx/", Order=0)]
        public ReCServices.WS_Trackjack_Service.PosicionesIndividualRequestBody Body;
        
        public PosicionesIndividualRequest() {
        }
        
        public PosicionesIndividualRequest(ReCServices.WS_Trackjack_Service.PosicionesIndividualRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.trackjack.mx/")]
    public partial class PosicionesIndividualRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string usuario;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string contrasena;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string Economico;
        
        public PosicionesIndividualRequestBody() {
        }
        
        public PosicionesIndividualRequestBody(string usuario, string contrasena, string Economico) {
            this.usuario = usuario;
            this.contrasena = contrasena;
            this.Economico = Economico;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class PosicionesIndividualResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="PosicionesIndividualResponse", Namespace="http://www.trackjack.mx/", Order=0)]
        public ReCServices.WS_Trackjack_Service.PosicionesIndividualResponseBody Body;
        
        public PosicionesIndividualResponse() {
        }
        
        public PosicionesIndividualResponse(ReCServices.WS_Trackjack_Service.PosicionesIndividualResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.trackjack.mx/")]
    public partial class PosicionesIndividualResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public ReCServices.WS_Trackjack_Service.TrackJackData[] PosicionesIndividualResult;
        
        public PosicionesIndividualResponseBody() {
        }
        
        public PosicionesIndividualResponseBody(ReCServices.WS_Trackjack_Service.TrackJackData[] PosicionesIndividualResult) {
            this.PosicionesIndividualResult = PosicionesIndividualResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class PosicionesTodosRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="PosicionesTodos", Namespace="http://www.trackjack.mx/", Order=0)]
        public ReCServices.WS_Trackjack_Service.PosicionesTodosRequestBody Body;
        
        public PosicionesTodosRequest() {
        }
        
        public PosicionesTodosRequest(ReCServices.WS_Trackjack_Service.PosicionesTodosRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.trackjack.mx/")]
    public partial class PosicionesTodosRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string usuario;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string contrasena;
        
        public PosicionesTodosRequestBody() {
        }
        
        public PosicionesTodosRequestBody(string usuario, string contrasena) {
            this.usuario = usuario;
            this.contrasena = contrasena;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class PosicionesTodosResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="PosicionesTodosResponse", Namespace="http://www.trackjack.mx/", Order=0)]
        public ReCServices.WS_Trackjack_Service.PosicionesTodosResponseBody Body;
        
        public PosicionesTodosResponse() {
        }
        
        public PosicionesTodosResponse(ReCServices.WS_Trackjack_Service.PosicionesTodosResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.trackjack.mx/")]
    public partial class PosicionesTodosResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public ReCServices.WS_Trackjack_Service.TrackJackData[] PosicionesTodosResult;
        
        public PosicionesTodosResponseBody() {
        }
        
        public PosicionesTodosResponseBody(ReCServices.WS_Trackjack_Service.TrackJackData[] PosicionesTodosResult) {
            this.PosicionesTodosResult = PosicionesTodosResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ServiceSoapChannel : ReCServices.WS_Trackjack_Service.ServiceSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServiceSoapClient : System.ServiceModel.ClientBase<ReCServices.WS_Trackjack_Service.ServiceSoap>, ReCServices.WS_Trackjack_Service.ServiceSoap {
        
        public ServiceSoapClient() {
        }
        
        public ServiceSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ServiceSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ReCServices.WS_Trackjack_Service.PosicionesIndividualResponse ReCServices.WS_Trackjack_Service.ServiceSoap.PosicionesIndividual(ReCServices.WS_Trackjack_Service.PosicionesIndividualRequest request) {
            return base.Channel.PosicionesIndividual(request);
        }
        
        public ReCServices.WS_Trackjack_Service.TrackJackData[] PosicionesIndividual(string usuario, string contrasena, string Economico) {
            ReCServices.WS_Trackjack_Service.PosicionesIndividualRequest inValue = new ReCServices.WS_Trackjack_Service.PosicionesIndividualRequest();
            inValue.Body = new ReCServices.WS_Trackjack_Service.PosicionesIndividualRequestBody();
            inValue.Body.usuario = usuario;
            inValue.Body.contrasena = contrasena;
            inValue.Body.Economico = Economico;
            ReCServices.WS_Trackjack_Service.PosicionesIndividualResponse retVal = ((ReCServices.WS_Trackjack_Service.ServiceSoap)(this)).PosicionesIndividual(inValue);
            return retVal.Body.PosicionesIndividualResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<ReCServices.WS_Trackjack_Service.PosicionesIndividualResponse> ReCServices.WS_Trackjack_Service.ServiceSoap.PosicionesIndividualAsync(ReCServices.WS_Trackjack_Service.PosicionesIndividualRequest request) {
            return base.Channel.PosicionesIndividualAsync(request);
        }
        
        public System.Threading.Tasks.Task<ReCServices.WS_Trackjack_Service.PosicionesIndividualResponse> PosicionesIndividualAsync(string usuario, string contrasena, string Economico) {
            ReCServices.WS_Trackjack_Service.PosicionesIndividualRequest inValue = new ReCServices.WS_Trackjack_Service.PosicionesIndividualRequest();
            inValue.Body = new ReCServices.WS_Trackjack_Service.PosicionesIndividualRequestBody();
            inValue.Body.usuario = usuario;
            inValue.Body.contrasena = contrasena;
            inValue.Body.Economico = Economico;
            return ((ReCServices.WS_Trackjack_Service.ServiceSoap)(this)).PosicionesIndividualAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ReCServices.WS_Trackjack_Service.PosicionesTodosResponse ReCServices.WS_Trackjack_Service.ServiceSoap.PosicionesTodos(ReCServices.WS_Trackjack_Service.PosicionesTodosRequest request) {
            return base.Channel.PosicionesTodos(request);
        }
        
        public ReCServices.WS_Trackjack_Service.TrackJackData[] PosicionesTodos(string usuario, string contrasena) {
            ReCServices.WS_Trackjack_Service.PosicionesTodosRequest inValue = new ReCServices.WS_Trackjack_Service.PosicionesTodosRequest();
            inValue.Body = new ReCServices.WS_Trackjack_Service.PosicionesTodosRequestBody();
            inValue.Body.usuario = usuario;
            inValue.Body.contrasena = contrasena;
            ReCServices.WS_Trackjack_Service.PosicionesTodosResponse retVal = ((ReCServices.WS_Trackjack_Service.ServiceSoap)(this)).PosicionesTodos(inValue);
            return retVal.Body.PosicionesTodosResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<ReCServices.WS_Trackjack_Service.PosicionesTodosResponse> ReCServices.WS_Trackjack_Service.ServiceSoap.PosicionesTodosAsync(ReCServices.WS_Trackjack_Service.PosicionesTodosRequest request) {
            return base.Channel.PosicionesTodosAsync(request);
        }
        
        public System.Threading.Tasks.Task<ReCServices.WS_Trackjack_Service.PosicionesTodosResponse> PosicionesTodosAsync(string usuario, string contrasena) {
            ReCServices.WS_Trackjack_Service.PosicionesTodosRequest inValue = new ReCServices.WS_Trackjack_Service.PosicionesTodosRequest();
            inValue.Body = new ReCServices.WS_Trackjack_Service.PosicionesTodosRequestBody();
            inValue.Body.usuario = usuario;
            inValue.Body.contrasena = contrasena;
            return ((ReCServices.WS_Trackjack_Service.ServiceSoap)(this)).PosicionesTodosAsync(inValue);
        }
    }
}
