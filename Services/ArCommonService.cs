using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Alexr03.Common.TCAdmin.Objects;
using Alexr03.Common.TCAdmin.Proxy;
using TCAdmin.Interfaces.Logging;
using TCAdmin.Interfaces.Server;
using TCAdmin.SDK;
using TCAdmin.SDK.Objects;

namespace Alexr03.Common.TCAdmin.Services
{
    public class ArCommonService : IMonitorService
    {
        private readonly Logging.LogManager _logger = Logging.LogManager.Create<ArCommonService>();
        public ArCommonService()
        {
            ConfigurationKey = "Alexr03.Common";
        }

        public void Initialize(params object[] args)
        {
            
        }

        public void Start()
        {
            Status = ServiceStatus.Starting;
            LogManager.Write("Starting Alexr03.Common Service...", LogType.Console);
            RegisterProxiesFromDatabase();
            RegisterOntoRemotes();
            LogManager.Write("Alexr03.Common Service has successfully started!", LogType.Console);
            Status = ServiceStatus.Running;
        }

        private void RegisterProxiesFromDatabase()
        {
            ProxyManager.RegisterFromAssembly(Assembly.GetExecutingAssembly());
            foreach (var assemblyProxy in AssemblyProxy.GetAssemblyProxies())
            {
                try
                {
                    _logger.Information("Loading proxies from - " + assemblyProxy.Assembly.FullName);
                    ProxyManager.RegisterFromAssembly(assemblyProxy.Assembly);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        private void RegisterOntoRemotes()
        {
            var serverIdAppSetting = Utility.AppSettings["TCAdmin.Monitor.ServerId"];
            if (!string.IsNullOrEmpty(serverIdAppSetting))
            {
                var serverId = int.Parse(serverIdAppSetting);
                if (serverId != 1)
                {
                    _logger.Information("Skipping registration on remotes");
                }
                else
                {
                    foreach (var server in Server.GetEnabledServers().Cast<Server>())
                    {
                        var serverEnabledComponents = ServerEnabledComponent.GetServerComponents(server.ServerId)
                            .Cast<ServerEnabledComponent>().ToList();
                        if (serverEnabledComponents.Any(x => x.ModuleId == Globals.ModuleId && x.ComponentId == 1))
                        {
                            _logger.Information(
                                $"Skipping registration of Alexr03.Common onto {server.Name} as it is already registered.");
                            continue;
                        }

                        _logger.Information("Registering Alexr03.Common onto " + server.Name);
                        var serverEnabledComponent = new ServerEnabledComponent()
                        {
                            ModuleId = Globals.ModuleId,
                            ComponentId = 1,
                            ServerId = server.ServerId
                        };
                        serverEnabledComponent.Save();
                    }
                }
            }
        }

        public void Stop()
        {
            Status = ServiceStatus.Stopping;
            LogManager.Write("Stopping Alexr03.Common Service...", LogType.Console);
            ProxyManager.UnRegisterProxies();
            LogManager.Write("Alexr03.Common Service has successfully stopped!", LogType.Console);
            Status = ServiceStatus.Stopped;
        }

        public void Pause()
        {
            throw new System.NotImplementedException();
        }

        public void Resume()
        {
            throw new System.NotImplementedException();
        }

        public void Restart()
        {
            Stop();
            Start();
        }

        public string ConfigurationKey { get; }
        public ServiceStatus Status { get; set; }
    }
}