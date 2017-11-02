using System;
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

namespace ReCServices.Apis
{
    public class MonitoreoYRastereo
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static async void MonitoreoYRastreo_ObtenerPosicion(string UsuarioReC, string Usuario, string Password)
        {
            var ECOenCurso = "";
            DataTable DT_Data = new DataTable();
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
                        //XmlDocument doc = new XmlDocument();
                        //doc.Load(@"C:\documents\projects\test_ws.txt");
                        //string xmlcontents = doc.InnerXml;

                        string xmlcontents = File.ReadAllText(@"C:\documents\projects\test_ws.txt");
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
                                //Console.WriteLine(soapResult);
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
    }
}