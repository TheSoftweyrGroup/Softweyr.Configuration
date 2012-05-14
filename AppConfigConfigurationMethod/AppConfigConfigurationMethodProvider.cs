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
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(propertyInfo.PropertyType);

            if (attribute is ConfigureUsingAppConfigAppSettingAttribute)
            {
                var configureUsingAppConfigAppSettingAttribute = attribute as ConfigureUsingAppConfigAppSettingAttribute;
                var appSettingName = configureUsingAppConfigAppSettingAttribute.AppSettingName;
                if (string.IsNullOrWhiteSpace(appSettingName))
                {
                    appSettingName = propertyInfo.Name;
                }

                object value;
                if (appConfig.AppSettings.Settings.AllKeys.Any<string>(new System.Func<string, bool>(key => key.Equals(appSettingName, StringComparison.OrdinalIgnoreCase))))
                {
                    value = appConfig.AppSettings.Settings[appSettingName].Value;
                }
                else if (machineConfig.AppSettings.Settings.AllKeys.Any<string>(new System.Func<string, bool>(key => key.Equals(appSettingName, StringComparison.OrdinalIgnoreCase))))
                {
                    value = machineConfig.AppSettings.Settings[appSettingName].Value;
                }
                else
                {
                    return false;
                }
                
                var convertedValue = converter.ConvertFrom(value);
                propertyInfo.SetValue(configurationInstance, convertedValue, null);
                return true;
            }

            return false;
        }
    }
}
