using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Alexr03.Common.TCAdmin.Proxy.Requests;
using Newtonsoft.Json;
using TCAdmin.SDK.Misc;
using TCAdmin.SDK.Proxies;
using TCAdmin.SDK.Web.References.ModuleApiGateway;
using AppDomainManager = TCAdmin.SDK.AppDomainManager;
using LogManager = Alexr03.Common.TCAdmin.Logging.LogManager;
using Server = TCAdmin.GameHosting.SDK.Objects.Server;

namespace Alexr03.Common.TCAdmin.Proxy
{
    public static class ProxyManager
    {
        private static readonly LogManager Logger = LogManager.Create(typeof(ProxyManager));
        public static readonly List<CommandProxy> CommandProxies = new List<CommandProxy>();

        private static void AddProxy(this CommandProxy commandProxy)
        {
            if (!CommandProxies.Contains(commandProxy)) CommandProxies.Add(commandProxy);
        }

        private static void RemoveProxy(this CommandProxy commandProxy)
        {
            if (CommandProxies.Contains(commandProxy)) CommandProxies.Remove(commandProxy);
        }

        private static void RemoveProxy(this string commandName)
        {
            if (CommandProxies.Any(x => x.CommandName == commandName))
                CommandProxies.RemoveAll(x => x.CommandName == commandName);
        }

        public static void RegisterProxies()
        {
            foreach (var commandProxy in CommandProxies) commandProxy.RegisterProxy();
        }

        public static void RegisterProxy(this CommandProxy commandProxy)
        {
            Logger.Information($"Registering {commandProxy.CommandName} proxy.");
            AppDomainManager.RegisterProxyCommand(commandProxy);
            AddProxy(commandProxy);
        }

        public static void UnRegisterProxies()
        {
            foreach (var commandProxy in CommandProxies.ToList())
            {
                UnRegisterProxy(commandProxy.CommandName);
            }

            CommandProxies.Clear();
        }

        public static void UnRegisterProxy(string commandName)
        {
            Logger.Information($"Unregistering {commandName} proxy.");
            AppDomainManager.UnregisterProxyCommand(commandName);
            RemoveProxy(commandName);
        }

        public static void RegisterFromAssembly(Assembly assembly)
        {
            var proxyRequests = assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(ProxyRequest)))
                .Select(t => (ProxyRequest) Activator.CreateInstance(t)).ToList();
            foreach (var request in proxyRequests)
            {
                new CommandProxy(request.Execute)
                {
                    CommandName = request.CommandName, KeepAlive = true, NeverDie = true
                }.RegisterProxy();
            }
        }

        public static T Request<T>(string commandName, Server server,
            object arguments, out CommandResponse commandResponse, bool waitForResponse = true,
            ProxyRequestType requestType = ProxyRequestType.Xml, JsonSerializerSettings settings = null)
        {
            try
            {
                commandResponse = new CommandResponse();
                if (requestType == ProxyRequestType.Json)
                {
                    arguments = JsonConvert.SerializeObject(arguments);
                }
                if (!server.ModuleApiGateway.ExecuteModuleCommand(commandName, arguments, ref commandResponse,
                    waitForResponse)) throw new Exception("Proxy command execution failed.");
                switch (requestType)
                {
                    case ProxyRequestType.Xml:
                    {
                        var xmlToObject = (T) ObjectXml.XmlToObject(commandResponse.Response.ToString(), typeof(T));
                        return xmlToObject;
                    }
                    case ProxyRequestType.Json:
                        if (settings == null) settings = Utilities.NoErrorJsonSettings;

                        Console.WriteLine("Respoinse = " + commandResponse.Response);
                        return JsonConvert.DeserializeObject<T>(commandResponse.Response.ToString());
                    default:
                        throw new ArgumentOutOfRangeException(nameof(requestType), requestType, null);
                }
            }
            catch (Exception e)
            {
                Logger.Fatal(e);
                commandResponse = new CommandResponse {SerializedException = e.Message};
                return Activator.CreateInstance<T>();
            }
        }
    }

    public enum ProxyRequestType
    {
        Xml,
        Json
    }
}