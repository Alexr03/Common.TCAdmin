using System;
using Newtonsoft.Json;
using TCAdmin.GameHosting.SDK.Objects;
using TCAdmin.SDK.Web.References.ModuleApiGateway;

namespace Alexr03.Common.TCAdmin.Proxy.Requests
{
    [Serializable]
    public abstract class ProxyRequest
    {
        [JsonIgnore] public abstract string CommandName { get; }

        public abstract object Execute(object arguments);
        
        public virtual T Request<T>(object arguments, out CommandResponse commandResponse,
            ProxyRequestType requestType = ProxyRequestType.Xml)
        {
            return ProxyManager.Request<T>(CommandName, Server.GetServerFromCache(1), arguments, out commandResponse, requestType: requestType);
        }

        public virtual T Request<T>(object arguments, Server server, out CommandResponse commandResponse,
            ProxyRequestType requestType = ProxyRequestType.Xml)
        {
            return ProxyManager.Request<T>(CommandName, server, arguments, out commandResponse, requestType: requestType);
        }
        
        public virtual T Request<T>(object arguments, int serverId, out CommandResponse commandResponse,
            ProxyRequestType requestType = ProxyRequestType.Xml)
        {
            return ProxyManager.Request<T>(CommandName, Server.GetServerFromCache(serverId), arguments, out commandResponse, requestType: requestType);
        }
        
        public virtual void Request(object arguments, int serverId, out CommandResponse commandResponse,
            ProxyRequestType requestType = ProxyRequestType.Xml)
        {
            ProxyManager.Request<object>(CommandName, new Server(serverId), arguments, out commandResponse, requestType: requestType);
        }
    }
}