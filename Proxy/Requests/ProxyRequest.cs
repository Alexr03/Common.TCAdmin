using System;
using Newtonsoft.Json;
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
            return ProxyManager.Request<T>(CommandName, arguments, out commandResponse, requestType: requestType);
        }
    }
}