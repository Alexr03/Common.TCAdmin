using System;
using System.Collections.Generic;
using System.Linq;
using Alexr03.Common.TCAdmin.Extensions;
using Newtonsoft.Json;
using TCAdmin.GameHosting.SDK.Automation;
using TCAdmin.Interfaces.Database;
using TCAdmin.SDK.Objects;

namespace Alexr03.Common.TCAdmin.Objects
{
    public class ModuleConfiguration : ObjectBase
    {
        public const string ViewKey = "AR_COMMON:ConfigurationView";
        public const string AlwaysPullFromTypeKey = "AR_COMMON:AlwaysPullFromType";

        public ModuleConfiguration()
        {
            this.TableName = "ar_common_configurations";
            this.KeyColumns = new[] {"id", "moduleId"};
            this.SetValue("id", -1);
            this.SetValue("moduleId", "");
            this.SetValue("contents", "{}");
            this.UseApplicationDataField = true;
        }

        public ModuleConfiguration(int id, string moduleId) : this()
        {
            this.SetValue("id", id);
            this.SetValue("moduleId", moduleId);
            this.ValidateKeys();
            if (!this.Find())
            {
                throw new KeyNotFoundException(
                    $"Could not find Module Configuration with Id: {id} | Module Id: {moduleId}");
            }
        }

        public int Id
        {
            get => this.GetIntegerValue("id");
            private set => this.SetValue("id", value);
        }

        public string ConfigName
        {
            get => this.GetStringValue("name");
            private set => this.SetValue("name", value);
        }

        public string ModuleId
        {
            get => this.GetStringValue("moduleId");
            private set => this.SetValue("moduleId", value);
        }

        private string Contents
        {
            get => this.GetStringValue("contents");
            set => this.SetValue("contents", value);
        }

        private string TypeName
        {
            get => this.GetStringValue("typeName");
            set => this.SetValue("typeName", value);
        }

        public Type Type => Type.GetType(TypeName);

        public bool HasView => View != null;

        public virtual string View
        {
            get => this.AppData.HasValueAndSet(ViewKey)
                ? this.AppData[ViewKey].ToString()
                : null;
            set => this.AppData[ViewKey] = value;
        }

        private void PreParse()
        {
            if (string.IsNullOrEmpty(Contents) || Contents == "{}" || (AppData.HasValueAndSet(AlwaysPullFromTypeKey) && bool.Parse(AppData[AlwaysPullFromTypeKey].ToString())))
            {
                SetConfiguration(Activator.CreateInstance(Type));
            }
        }

        public T Parse<T>()
        {
            PreParse();
            return JsonConvert.DeserializeObject<T>(Contents);
        }
        
        public object Parse(Type type)
        {
            PreParse();
            return JsonConvert.DeserializeObject(Contents, type);
        }
        
        public object Parse()
        {
            PreParse();
            return JsonConvert.DeserializeObject(Contents, Type);
        }

        public bool SetConfiguration(object config)
        {
            var serializeObject = JsonConvert.SerializeObject(config);
            TypeName = $"{config.GetType()}, {config.GetType().Assembly.GetName().Name}";
            Contents = serializeObject;
            return this.Save();
        }

        public static ModuleConfiguration GetModuleConfiguration(string moduleId, string configName, Type type = null)
        {
            var whereList = new WhereList
            {
                {nameof(moduleId), moduleId},
                {"name", configName},
            };

            var moduleConfigurations =
                new ModuleConfiguration().GetObjectList(whereList).Cast<ModuleConfiguration>().ToList();
            if (moduleConfigurations.Any())
            {
                var moduleConfiguration = moduleConfigurations[0];
                if (!string.IsNullOrEmpty(moduleConfiguration.TypeName) || type == null) return moduleConfiguration;

                moduleConfiguration.SetConfiguration(Activator.CreateInstance(type));
                moduleConfiguration.Save();

                return moduleConfiguration;
            }

            if (type == null)
            {
                throw new Exception(
                    $"Cannot find configuration with {nameof(moduleId)}={moduleId} | {nameof(configName)} = {configName} and cannot auto generate without Type specified.");
            }

            var newConfig = new ModuleConfiguration
            {
                ModuleId = moduleId,
                ConfigName = configName,
            };
            newConfig.GenerateKey();
            newConfig.Save();
            newConfig.SetConfiguration(Activator.CreateInstance(type));

            return newConfig;
        }

        public static List<ModuleConfiguration> GetModuleConfigurations(string moduleId)
        {
            var whereList = new WhereList
            {
                {nameof(moduleId), moduleId},
            };
            return GetModuleConfigurationsObjectList(whereList).Cast<ModuleConfiguration>().ToList();
        }
        
        public static ObjectList GetModuleConfigurationsObjectList(string moduleId)
        {
            var whereList = new WhereList
            {
                {nameof(moduleId), moduleId}
            };

            return GetModuleConfigurationsObjectList(whereList);
        }
        
        public static ObjectList GetModuleConfigurationsObjectList(WhereList whereList)
        {
            var moduleConfigurations = new ModuleConfiguration().GetObjectList(whereList);
            return moduleConfigurations;
        }
    }
}