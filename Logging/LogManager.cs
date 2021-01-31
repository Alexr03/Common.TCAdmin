using System;
using System.IO;
using Alexr03.Common.TCAdmin.Objects;
using Serilog;
using Serilog.Events;
using TCAdmin.SDK;

namespace Alexr03.Common.TCAdmin.Logging
{
    public class LogManager
    {
        private readonly string _logBaseLocation = Utility.GetLogPath() + "Components\\{0}\\{1}\\{2}\\{2}.log";
        public string Application { get; }
        private Type Type { get; }
        private string LogLocation { get; }
        public Serilog.Core.Logger InternalLogger { get; }

        public LogManager(string application, Type type = null)
        {
            try
            {
                Application = application;
                Type = type;

                var consoleOutputTemplate =
                    $"[{application}" + " {Timestamp:HH:mm:ss.ff} {Level:u3}] {Message:lj}{NewLine}{Exception}";
                var arCommonSettings = ModuleConfiguration.GetModuleConfiguration(Globals.ModuleId, "ArCommonSettings").Parse<ArCommonSettings>();
                var loggerConfiguration = new LoggerConfiguration()
                    .WriteTo.Console(outputTemplate: consoleOutputTemplate)
                    .MinimumLevel.Is(arCommonSettings.MinimumLogLevel);

                if (type != null)
                {
                    var assemblyName = Type.Assembly.GetName().Name;
                    LogLocation =
                        Path.Combine(
                            _logBaseLocation
                                .Replace("{0}", assemblyName)
                                .Replace("{1}", Type.Namespace?.Replace(assemblyName, "").Trim('.'))
                                .Replace("{2}", application));
                }
                else
                {
                    LogLocation =
                        Path.Combine(
                            _logBaseLocation
                                .Replace("{0}", "Unknown")
                                .Replace("{1}", application)
                                .Replace("{2}", application));
                }

                loggerConfiguration.WriteTo.File(LogLocation, rollingInterval: RollingInterval.Day, shared: true);
                InternalLogger = loggerConfiguration.CreateLogger();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
        }

        public static LogManager Create(Type type)
        {
            var logger = new LogManager(type.Name, type);
            return logger;
        }
        
        public static LogManager Create(string application, Type type)
        {
            var logger = new LogManager(application, type);
            return logger;
        }

        public static LogManager Create<T>(string application)
        {
            var logger = new LogManager(application, typeof(T));
            return logger;
        }

        public static LogManager Create<T>()
        {
            var logger = new LogManager(typeof(T).Name, typeof(T));
            return logger;
        }

        public void Information(string message)
        {
            LogMessage(message);
        }

        public void Debug(string message)
        {
            LogMessage(message, logEventLevel: LogEventLevel.Debug);
        }

        public void Warning(string message)
        {
            LogMessage(message, logEventLevel: LogEventLevel.Warning);
        }

        public void Verbose(string message)
        {
            LogMessage(message, logEventLevel: LogEventLevel.Verbose);
        }

        public void Error(string message)
        {
            LogMessage(message, logEventLevel: LogEventLevel.Error);
        }
        
        public void Fatal(string message)
        {
            LogMessage(message, logEventLevel: LogEventLevel.Fatal);
        }

        public void Fatal(string message, Exception e)
        {
            LogMessage(message, logEventLevel: LogEventLevel.Fatal);
        }

        public void Fatal(Exception e)
        {
            LogMessage(e.Message, logEventLevel: LogEventLevel.Fatal);
        }

        public void LogMessage(string message, Exception exception = null, LogEventLevel logEventLevel = LogEventLevel.Information)
        {
            if (exception != null)
            {
                InternalLogger.Fatal(exception, message);
            }
            InternalLogger.Write(logEventLevel, message);
        }
    }
}