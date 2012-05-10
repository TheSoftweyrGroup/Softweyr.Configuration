using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Softweyr.Configuration
{
    public class AppConfigConfigurationMethodProvider : IConfigurationMethodProvider
    {
        private readonly System.Configuration.Configuration appConfig;

        private readonly System.Configuration.Configuration machineConfig;

        public AppConfigConfigurationMethodProvider()
        {
            // TODO: Check ConfigurationUserLevel
            appConfig =
                ConfigurationManager.OpenExeConfiguration(
                ConfigurationUserLevel.None);
            machineConfig = ConfigurationManager.OpenMachineConfiguration();
        }

        public IEnumerable<Type> ConfigureAttributeTypesSupported
        {
            get { return new[] { typeof(ConfigureUsingAppConfigAppSettingAttribute) }; }
        }

        public bool TryPopulate(object configurationInstance, System.Reflection.PropertyInfo propertyInfo, ConfigureAttribute attribute)
        {
            if (attribute is ConfigureUsingAppConfigAppSettingAttribute)
            {
                var configureUsingAppConfigAppSettingAttribute = attribute as ConfigureUsingAppConfigAppSettingAttribute;
                var appSettingName = configureUsingAppConfigAppSettingAttribute.AppSettingName;
                if (string.IsNullOrWhiteSpace(appSettingName))
                {
                    appSettingName = propertyInfo.Name;
                }

                var value = appConfig.AppSettings.Settings[appSettingName];
                if (!appConfig.AppSettings.Settings.AllKeys.Any<string>(new System.Func<string, bool>(key => key.Equals(appSettingName, StringComparison.OrdinalIgnoreCase))))
                {
                    // TODO: Check if we need to bother doing this.
                    value = machineConfig.AppSettings.Settings[appSettingName];
                    if (!machineConfig.AppSettings.Settings.AllKeys.Any<string>(new System.Func<string, bool>(key => key.Equals(appSettingName, StringComparison.OrdinalIgnoreCase))))
                    {
                        return false;
                    }
                }

                propertyInfo.SetValue(configurationInstance, value, null);
                return true;
            }

            return false;
        }
    }
}
