using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace ReCServices
{
    public class GPSHub : Hub
    {
        private static readonly List<UserConnection> uList = new List<UserConnection>();

        //public void Hello()
        //{
        //    Clients.All.hello();
        //}
        public void send(string SenderUser, string Message, dynamic Obj = null)
        {
            //var x = UserHandler.ConnectedIds;
            Clients.All.broadCastMessage(SenderUser, Message, Obj);
        }
        public void SendToAll(string SenderUser, string Message, dynamic Obj = null)
        {
            //var x = uList;
            Clients.All.broadCastMessage(SenderUser, Message, Obj);
        }
        public void SendToOthers(string SenderUser, string Message, dynamic Obj = null)
        {
            Clients.Others.broadCastMessage(SenderUser, Message, Obj);
        }
        public void SendToSpecificUser(string DestinationUser, string SenderUser, string Message, dynamic Obj = null)
        {
            Clients.Client(DestinationUser).broadcastMessage(SenderUser, Message, Obj);
        }
        public void SendToUserList(string[] UserList, string SenderUser, string Message, dynamic Obj = null)
        {
            Clients.Users(UserList).broadcastMessage(SenderUser, Message, Obj);
        }

        public override Task OnConnected()
        {
            //try
            //{
                var username = Context.QueryString["username"];

                if (username == null || username == "")
                {
                    //throw new HubException("Unauthorized: No se paso el nombre de usuario como querystring en la coneccion", new { status = "401" });
                    //throw new Exception("SIGNALR: No se paso el nombre de usuario como querystring en la coneccion");
                    //Clients.Caller.broadcastMessage("SISTEMA", "Unauthorized: No se paso el nombre de usuario como querystring en la coneccion");
                    //throw new HubException("SIGNALR: No se paso el nombre de usuario como querystring en la coneccion");
                    Clients.Client(Context.ConnectionId).serverOrderedDisconnect();
                };
                string name = Context.User.Identity.Name;
                UserConnection uc = new UserConnection();
                uc.UserName = username;
                uc.ConnectionID = Context.ConnectionId;
                uList.Add(uc);
                return base.OnConnected();
            //}
            //catch (Exception Ex)
            //{
            //    System.Diagnostics.Trace.TraceError(String.Format("An error has occured when tried to send back the last message. ConnectionID:{0} Error message: {1}", connectionId, Ex.Message));
            //}
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string name = Context.User.Identity.Name;

            var x = uList.Find(item => item.ConnectionID == Context.ConnectionId);
            uList.Remove(x);

            return base.OnDisconnected(stopCalled);
        }
    }
    public class UserConnection
    {
        public string UserName { set; get; }
        public string ConnectionID { set; get; }
    }

}