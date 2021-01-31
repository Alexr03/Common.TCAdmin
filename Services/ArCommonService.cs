using System;
using System.IO;
using System.Reflection;
using Alexr03.Common.TCAdmin.Objects;
using Alexr03.Common.TCAdmin.Proxy;
using TCAdmin.Interfaces.Logging;
using TCAdmin.Interfaces.Server;
using TCAdmin.SDK;

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
            LogManager.Write("Alexr03.Common Service has successfully started!", LogType.Console);
            Status = ServiceStatus.Running;
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