using System;
using Alexr03.Common.Configuration;
using Alexr03.Common.Logging;
using Alexr03.Common.TCAdmin.Objects;

namespace Alexr03.Common.TCAdmin.Configuration
{
    public class DatabaseConfiguration<T> : ConfigurationProvider<T>
    {
        private readonly Logger _logger = Logger.Create<DatabaseConfiguration<T>>();
        private readonly string _moduleId;

        public DatabaseConfiguration(string moduleId, string configName) : base(configName)
        {
            _moduleId = moduleId;
            ConfigName = ConfigName.Replace(".json", "");
        }

        public override T GetConfiguration()
        {
            var moduleConfiguration = ModuleConfiguration.GetModuleConfiguration(_moduleId, ConfigName, typeof(T));
            try
            {
                return moduleConfiguration.Parse<T>();
            }
            catch
            {
                return GetTObject();
            }
        }

        public override bool SetConfiguration(T config)
        {
            try
            {
                var moduleConfiguration = ModuleConfiguration.GetModuleConfiguration(_moduleId, ConfigName, typeof(T));
                moduleConfiguration.SetConfiguration(config);
                moduleConfiguration.Save();

                return true;
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return false;
            }
        }
    }
}