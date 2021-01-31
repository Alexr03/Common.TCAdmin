using System;
using TCAdmin.SDK.Integration;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Alexr03.Common.TCAdmin.Proxy
{
    public class ProxyResponse<T> : ProxyResponse
    {
        public T ReturnData { get; }

        public ProxyResponse()
        {
        }

        public ProxyResponse(string moduleId, ReturnStatus returnStatus) : base(moduleId, returnStatus)
        {
        }

        public ProxyResponse(string moduleId, ReturnStatus returnStatus, Exception exception) : base(moduleId, returnStatus, exception)
        {
        }

        public ProxyResponse(string moduleId, ReturnStatus returnStatus, T returnData) : base(moduleId, returnStatus)
        {
            ReturnData = returnData;
        }

        public ProxyResponse(string moduleId, ReturnStatus returnStatus, Exception exception, T returnData) : base(moduleId, returnStatus, exception)
        {
            ReturnData = returnData;
        }
    }

    public class ProxyResponse
    {
        public string ModuleId { get; }
        public ReturnStatus ReturnStatus { get; }

        public Exception Exception { get; }

        public ProxyResponse()
        {
        }

        public ProxyResponse(string moduleId, ReturnStatus returnStatus)
        {
            ModuleId = moduleId;
            ReturnStatus = returnStatus;
        }

        public ProxyResponse(string moduleId, ReturnStatus returnStatus, Exception exception) : this(moduleId, returnStatus)
        {
            Exception = exception;
        }
    }
}