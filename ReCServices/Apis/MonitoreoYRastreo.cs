using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Net.Http;

using System.Xml.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Xml;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
//using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using System.Xml.Serialization;

namespace ReCServices.Apis
{
    public class MonitoreoYRastereo
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static async void MonitoreoYRastreo_ObtenerPosicion(string UsuarioReC, string Usuario, string Password)
        {
            var ECOenCurso = "";
            DataTable DT_Data = new DataTable();

            //var ENV = "<soapenv:envelope xmlns:quer=\"query.php\"xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\"xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"><soapenv:header><soapenv:body><quer:ultimaposicion soapenv:encodingstyle=\"http://schemas.xmlsoap.org/soap/encoding/\"><unitplate xsi:type=\"xsd:string\">[-placa-]</unitplate><usuario xsi:type=\"xsd:string\">[-user-]</usuario><password xsi:type=\"xsd:string\">[-pwd-]</password></quer:ultimaposicion></soapenv:body></soapenv:envelope>";



            try
            {
                //Carga los IMEI que se van a consultar
                DT_Data = GetData_ListaGPSxProveedor("ETI"); //tmp para no modificar demaciado el codigo (x8)
                if (DT_Data.Rows.Count == 0)
                {
                    return;
                }

                for (int i = 0; i < DT_Data.Rows.Count; i++)
                {
                    ECOenCurso = DT_Data.Rows[i]["Economico"].ToString();

                    var responseJson = "";
                    try
                    {
                        string sPath = "";
                        sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/" + "SOAPENV\\SOAPENV_DIEZ.txt");

                        //XmlDocument doc = new XmlDocument();
                        //doc.Load(@"\test_ws.txt");
                        //string xmlcontents = doc.InnerXml;
                        string xmlcontents = File.ReadAllText(sPath);
                        xmlcontents = xmlcontents.Replace("[-placa-]", "933AT7");//ECOenCurso);
                        xmlcontents = xmlcontents.Replace("[-pwd-]", Password);
                        xmlcontents = xmlcontents.Replace("[-user-]", Usuario);


                        HttpWebRequest request = CreateWebRequest();
                        XmlDocument soapEnvelopeXml = new XmlDocument();
                        soapEnvelopeXml.LoadXml(xmlcontents);

                        using (Stream stream = request.GetRequestStream())
                        {
                            soapEnvelopeXml.Save(stream);
                        }

                        using (WebResponse response = request.GetResponse())
                        {
                            using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                            {
                                string soapResult = rd.ReadToEnd();

                                
                                
                                XmlDocument document = new XmlDocument();
                                document.LoadXml(soapResult);
                                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(SOAPClass.Envelope));
                                SOAPClass.Envelope envelope = (SOAPClass.Envelope)serializer.Deserialize(new StringReader(document.OuterXml));


                                var rawXML = XDocument.Parse(soapResult);
                                SOAPClass.Envelope deserializedObject;
                                using (var reader = rawXML.CreateReader(System.Xml.Linq.ReaderOptions.None))
                                {
                                    var ser = new XmlSerializer(typeof(SOAPClass.Envelope));
                                    deserializedObject = (SOAPClass.Envelope)ser.Deserialize(reader);
                                    var z = deserializedObject;
                                }

                                return;
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
                            log.Error("Error GrupoUDA_ObtenerPosicion: " + UsuarioReC + ". " + responseJson + ". " + Ex.Message);
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
                    log.Error("Error GrupoGCP_ObtenerPosicion: " + UsuarioReC + ". " + Ex.Message);
                }
            }
        }

        public static T SOAPToObject<T>(string SOAP)
        {
            if (string.IsNullOrEmpty(SOAP))
            {
                throw new ArgumentException("SOAP can not be null/empty");
            }
            using (MemoryStream Stream = new MemoryStream(UTF8Encoding.UTF8.GetBytes(SOAP)))
            {
                SoapFormatter Formatter = new SoapFormatter();
                return (T)Formatter.Deserialize(Stream);
            }
        }


        public static DataTable GetData_ListaGPSxProveedor(string Proveedor)
        {
            DataTable dtbl = new DataTable("DataTable1");

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WS_PROD"].ConnectionString);
            SqlDataAdapter adp = new SqlDataAdapter();
            adp.SelectCommand = new SqlCommand();
            adp.SelectCommand.Connection = con;

            adp.SelectCommand.CommandText = "WS_GPS_ListaGPSxProveedor";
            adp.SelectCommand.CommandType = CommandType.StoredProcedure;
            adp.SelectCommand.Parameters.AddWithValue("@IdTransportista", 0);
            adp.SelectCommand.Parameters.AddWithValue("@Proveedor", Proveedor);
            adp.Fill(dtbl);

            return dtbl;
        }

        public static HttpWebRequest CreateWebRequest()
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(@" http://45.55.209.120/sistema/soap/query.php?wsdl");
            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }



        /// <comentarios/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
        public partial class Envelope
        {

            private EnvelopeBody bodyField;

            private string encodingStyleField;

            /// <comentarios/>
            public EnvelopeBody Body
            {
                get
                {
                    return this.bodyField;
                }
                set
                {
                    this.bodyField = value;
                }
            }

            /// <comentarios/>
            [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
            public string encodingStyle
            {
                get
                {
                    return this.encodingStyleField;
                }
                set
                {
                    this.encodingStyleField = value;
                }
            }
        }

        /// <comentarios/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public partial class EnvelopeBody
        {

            private UltimaPosicionResponse ultimaPosicionResponseField;

            /// <comentarios/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "query.php")]
            public UltimaPosicionResponse UltimaPosicionResponse
            {
                get
                {
                    return this.ultimaPosicionResponseField;
                }
                set
                {
                    this.ultimaPosicionResponseField = value;
                }
            }
        }

        /// <comentarios/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "query.php")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "query.php", IsNullable = false)]
        public partial class UltimaPosicionResponse
        {

            private @return returnField;

            /// <comentarios/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "")]
            public @return @return
            {
                get
                {
                    return this.returnField;
                }
                set
                {
                    this.returnField = value;
                }
            }
        }

        /// <comentarios/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class @return
        {

            private returnItem itemField;

            private string arrayTypeField;

            /// <comentarios/>
            public returnItem item
            {
                get
                {
                    return this.itemField;
                }
                set
                {
                    this.itemField = value;
                }
            }

            /// <comentarios/>
            [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://schemas.xmlsoap.org/soap/encoding/")]
            public string arrayType
            {
                get
                {
                    return this.arrayTypeField;
                }
                set
                {
                    this.arrayTypeField = value;
                }
            }
        }

        /// <comentarios/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class returnItem
        {

            private byte respuestaField;

            private string unitPlateField;

            private string unitldField;

            private decimal latitudeField;

            private decimal longitudeField;

            private byte altitudeField;

            private decimal odometerField;

            private byte speedGpsField;

            private ushort courseField;

            private decimal oilField;

            private bool ignitionField;

            private System.DateTime dateGpsField;

            private ulong idgpsField;

            private bool panicButtonField;

            /// <comentarios/>
            public byte Respuesta
            {
                get
                {
                    return this.respuestaField;
                }
                set
                {
                    this.respuestaField = value;
                }
            }

            /// <comentarios/>
            public string UnitPlate
            {
                get
                {
                    return this.unitPlateField;
                }
                set
                {
                    this.unitPlateField = value;
                }
            }

            /// <comentarios/>
            public string Unitld
            {
                get
                {
                    return this.unitldField;
                }
                set
                {
                    this.unitldField = value;
                }
            }

            /// <comentarios/>
            public decimal Latitude
            {
                get
                {
                    return this.latitudeField;
                }
                set
                {
                    this.latitudeField = value;
                }
            }

            /// <comentarios/>
            public decimal Longitude
            {
                get
                {
                    return this.longitudeField;
                }
                set
                {
                    this.longitudeField = value;
                }
            }

            /// <comentarios/>
            public byte Altitude
            {
                get
                {
                    return this.altitudeField;
                }
                set
                {
                    this.altitudeField = value;
                }
            }

            /// <comentarios/>
            public decimal Odometer
            {
                get
                {
                    return this.odometerField;
                }
                set
                {
                    this.odometerField = value;
                }
            }

            /// <comentarios/>
            public byte SpeedGps
            {
                get
                {
                    return this.speedGpsField;
                }
                set
                {
                    this.speedGpsField = value;
                }
            }

            /// <comentarios/>
            public ushort Course
            {
                get
                {
                    return this.courseField;
                }
                set
                {
                    this.courseField = value;
                }
            }

            /// <comentarios/>
            public decimal Oil
            {
                get
                {
                    return this.oilField;
                }
                set
                {
                    this.oilField = value;
                }
            }

            /// <comentarios/>
            public bool Ignition
            {
                get
                {
                    return this.ignitionField;
                }
                set
                {
                    this.ignitionField = value;
                }
            }

            /// <comentarios/>
            public System.DateTime DateGps
            {
                get
                {
                    return this.dateGpsField;
                }
                set
                {
                    this.dateGpsField = value;
                }
            }

            /// <comentarios/>
            public ulong idgps
            {
                get
                {
                    return this.idgpsField;
                }
                set
                {
                    this.idgpsField = value;
                }
            }

            /// <comentarios/>
            public bool PanicButton
            {
                get
                {
                    return this.panicButtonField;
                }
                set
                {
                    this.panicButtonField = value;
                }
            }
        }


    }
}