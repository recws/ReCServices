using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace ReCServices.Apis
{
    public class PlataformaLVT
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        public static async void PlataformaLVT_ObtenerPosicion(string UsuarioReC, string Usuario, string Password, string vehiclegroup, string TProveedor)
        {                        
            var IMEIenCurso = "";
            DataTable DT_Data = new DataTable();

            try
            {
                //Solicita todos los vehiculos de un grupo en especifico {vehiclegroupId}
                //En esta URL se obtienen los grupos de vehiculos
                //http://us.mzoneweb.net/api/v2/vehiclegroups.json
                //JSon resultado de solicitar los vehiculos de un grupo en especifico
                //"Items":[{"Id":37599583897,"UnitId":"2051794","Direction":345.00,"EventTypeId":1,"EventTypeDescription":"Posición Periódica","LocalTimestamp":"2017-11-17T06:35:09-05:00","Odometer":40345,"Position":[-100.46444,22.95146],"RPM":0,"Speed":102.00,"UnitOfDistanceCode":"km"}]
                
                var responseJson = "";
                var responseJson2 = "";
                var responseJson3 = "";

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://us.mzoneweb.net");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var byteArray = Encoding.ASCII.GetBytes(Usuario + ":" + Password);
                    var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    client.DefaultRequestHeaders.Authorization = header;

                    //var fini = DateTime.Now.AddMinutes(-5).ToString("yyyyMMddTHHmmss");
                    var fini = DateTime.Now.AddHours(-5).ToString("yyyyMMddTHHmmss");                    
                    var ffin = DateTime.Now.ToString("yyyyMMddTHHmmss");

                    HttpResponseMessage responseMessage = await client.GetAsync("/api/v2/vehiclegroups/"+ vehiclegroup + "/events/" + fini + "/" + ffin + ".json");
                                        
                    responseJson = await responseMessage.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<Eventos>(responseJson);


                    //Solicita la ultima posicion en del grupo de vehiculos en un rango de -5 y la fecha y hora actual 
                    //Aqui se puede consultar mas datos de los vehiculos por grupo
                    //https://us.mzoneweb.net/api/v2/vehiclegroups/{vehiclegroupId}/units.json
                    //JSon resultado de solicitar las ultimas posiciones de un grupo en especifico
                    //"Items":[{"LocalTimestamp":"2017-11-17T15:04:26-05:00","Id":"4cf8f7b2-df0e-4c09-a093-2352cb0dc932","Description":"3163 Jose Victoria Medrano","UnitId":"1454100","Position":[-99.30786,19.34838],"Location":"Carretera Toluca - México, Zentlapatl, Ciudad de México"}]

                    using (var client2 = new HttpClient())
                    {
                        client2.BaseAddress = new Uri("https://us.mzoneweb.net");
                        client2.DefaultRequestHeaders.Accept.Clear();
                        client2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var byteArray2 = Encoding.ASCII.GetBytes(Usuario + ":" + Password);
                        var header2 = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray2));
                        client2.DefaultRequestHeaders.Authorization = header2;
                    

                        HttpResponseMessage responseMessage2 = await client2.GetAsync("/api/v2/vehiclegroups/" + vehiclegroup + "/lastknownpositions.json");
                        
                        responseJson2 = await responseMessage2.Content.ReadAsStringAsync();

                        var result2 = JsonConvert.DeserializeObject<Eventos>(responseJson2);

                        //////// busca por unitid y fechahora para obtener velocidad, odometro y heading
                        for (int i = 0; i < result2.Items.Count; i++) //Result2 es el array con elementos unicos
                        {
                            for (int j = 0; j < result.Items.Count; j++)
                            {
                                if (result.Items[j].UnitId == result2.Items[i].UnitId && result.Items[j].LocalTimestamp == result2.Items[i].LocalTimestamp)
                                {
                                    result2.Items[i].Speed = result.Items[j].Speed;
                                    result2.Items[i].Odometer = result.Items[j].Odometer;
                                    result2.Items[i].Direction = result.Items[j].Direction;
                                    result2.Items[i].EventTypeId = result.Items[j].EventTypeId;
                                    result2.Items[i].EventTypeDescription = result.Items[j].EventTypeDescription;
                                    break;
                                }
                            }
                        }


                        for (int i = 0; i < result2.Items.Count; i++)
                        {
                            try
                            {


                                using (var client3 = new HttpClient())
                                {
                                    client3.BaseAddress = new Uri("https://us.mzoneweb.net");
                                    client3.DefaultRequestHeaders.Accept.Clear();
                                    client3.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                                    var byteArray3 = Encoding.ASCII.GetBytes(Usuario + ":" + Password);
                                    var header3 = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray3));
                                    client3.DefaultRequestHeaders.Authorization = header3;


                                    HttpResponseMessage responseMessage3 = await client3.GetAsync("/api/v2/vehicle/" + result2.Items[i].Id.ToString() + "/vehicle.json");

                                    responseJson3 = await responseMessage3.Content.ReadAsStringAsync();

                                    var result3 = JsonConvert.DeserializeObject<RootObject>(responseJson3);

                                    //Quita las guines (-) de la placa
                                    result3.Registration = System.Text.RegularExpressions.Regex.Replace(result3.Registration, "-", "");
                                    
                                    DT_Data = GetData_ListaGPSxProveedor(TProveedor); //tmp para no modificar demasiado el codigo (x8)                                

                                    string imei = result3.Registration.ToString();
                                    
                                    for (int k = 0; k < DT_Data.Rows.Count; k++)
                                    {
                                        IMEIenCurso = DT_Data.Rows[k]["IMEI"].ToString();

                                        //Hacer una peticion por cada ID para conocer las placas, si estan en el arreglo insertar

                                        if (imei != IMEIenCurso)
                                        {
                                            continue;
                                        }

                                        string codigoevento = result2.Items[i].EventTypeId.ToString();
                                        string lat = result2.Items[i].Position[1].ToString();
                                        string lng = result2.Items[i].Position[0].ToString();
                                        string odometro = result2.Items[i].Odometer.ToString().Split('.')[0];
                                        string velocidad = result2.Items[i].Speed.ToString().Split('.')[0];
                                        string bateria = "100";
                                        string direccion = result2.Items[i].Direction.ToString().Split('.')[0];

                                        DateTime fechahoragps = result2.Items[i].LocalTimestamp.ToUniversalTime();

                                        ////Validaciones

                                        ////Conversiones de datos
                                        var LAT = decimal.Parse(lat);
                                        var LNG = decimal.Parse(lng);
                                        var ODOMETRO = int.Parse(odometro);
                                        var VELOCIDAD = int.Parse(velocidad);
                                        var DIRECCION = int.Parse(direccion);
                                        var BATERIA = int.Parse(bateria);


                                        //Si no es repetida la inserta
                                        List<WS_GPS_InsertaSimple_Result> WS_GPS_InsertaSimple;

                                        WS_CONTEXT db = new WS_CONTEXT();

                                        WS_GPS_InsertaSimple = db.WS_GPS_InsertaSimple(UsuarioReC, imei, codigoevento, LAT, LNG, "", true, VELOCIDAD, DIRECCION, BATERIA, ODOMETRO, fechahoragps, fechahoragps).ToList();
                                        if (WS_GPS_InsertaSimple[0].Indicador == 1)
                                        {

                                        }
                                        else
                                        {
                                            log.Error("Error al Insertar evento de " + UsuarioReC + ". " + WS_GPS_InsertaSimple[0].Mensaje + ". " + imei);
                                        }
                                    }
                                }
                            }
                            catch (Exception Ex)
                            {
                                log.Error("Error PlataformaLVT_ObtenerPosicion: " + UsuarioReC + ". " + ". " + Ex.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                log.Error("Error PlataformaLVT_ObtenerPosicion: " + UsuarioReC + ". " + Ex.Message);
            }

        }


        public class Item
        {
            public object Id { get; set; }
            public string UnitId { get; set; }
            public double Direction { get; set; }
            public int EventTypeId { get; set; }
            public string EventTypeDescription { get; set; }
            public DateTime LocalTimestamp { get; set; }
            public int Odometer { get; set; }
            public IList<double> Position { get; set; }
            public int RPM { get; set; }
            public double Speed { get; set; }
            public string UnitOfDistanceCode { get; set; }
        }

        public class Eventos
        {
            public int Page { get; set; }
            public int PageSize { get; set; }
            public int TotalResults { get; set; }
            public IList<Item> Items { get; set; }
            public bool HasMoreResults { get; set; }
        }

        public class RootObject
        {
            public string Id { get; set; }
            public string Description { get; set; }
            public string Registration { get; set; }
            public bool IsFavourite { get; set; }
            public DateTime UtcLastUpdate { get; set; }
            public string Unit_Id { get; set; }
            public string VehicleType_Id { get; set; }
            public DateTime UtcStartDate { get; set; }
            public object UtcEndDate { get; set; }
            public int? ModelYear { get; set; }
            public DateTime CofDueDate { get; set; }
            public DateTime PurchaseDate { get; set; }
            public string Notes { get; set; }
            public bool IsMRMEnabled { get; set; }
            public bool IsFuelReportingEnabled { get; set; }
            public double MonthlyDistanceLimit { get; set; }
            public DateTime UtcLastModified { get; set; }
            public int Make_Id { get; set; }
            public string Make { get; set; }
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


    }
}