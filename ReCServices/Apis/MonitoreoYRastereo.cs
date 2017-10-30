using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace ReCServices.Apis
{
    public class MonitoreoYRastereo
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static async void MonitoreoYRastreo_ObtenerPosicion(string UsuarioReC, string Usuario, string Password)
        {
            var responseJson = "";
            try
            {
                using (var client = new HttpClient())
                {

                    client.BaseAddress = new Uri("http://45.55.209.120");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

                    //send request
                    //HttpResponseMessage responseMessage = await client.GetAsync("/sistema/soap/query.php/ultimaposicion?wsdl&op=UltimaPosicion&usuario=" + Usuario + "&clave=" + Password + "&placa=800WT6&op=UltimaPosicion");
                    //HttpResponseMessage responseMessage = await client.GetAsync("/sistema/soap/query.php?wsdl&op=UltimaPosicion&usuario=" + Usuario + "&clave=" + Password + "&placa=800WT6&op=UltimaPosicion");
                    HttpResponseMessage responseMessage = await client.GetAsync("/sistema/soap/query.php?wsdl&usuario=" + Usuario + "&clave=" + Password + "&placa=800WT6&op=UltimaPosicion");
                    //HttpResponseMessage responseMessage = await client.PostAsync("/web%20services/ws_last_position/ws_last_position.asmx/GetLastPosition_02?User=" + Usuario  + "&Password=" + Password + "&Page=", formContent);

                    //get access token from response body
                    responseJson = await responseMessage.Content.ReadAsStringAsync();



                   

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
}