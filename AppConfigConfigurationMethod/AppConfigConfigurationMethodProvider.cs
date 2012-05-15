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
            get { return new[] { typeof(ConfigureUsingAppConfigAppSettingAttribute), typeof(ConfigureUsingAppConfigConnectionStringAttribute) }; }
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

                object value;
                if (appConfig.AppSettings.Settings.AllKeys.Any(key => key.Equals(appSettingName, StringComparison.OrdinalIgnoreCase)))
                {
                    value = appConfig.AppSettings.Settings[appSettingName].Value;
                }
                else if (machineConfig.AppSettings.Settings.AllKeys.Any(key => key.Equals(appSettingName, StringComparison.OrdinalIgnoreCase)))
                {
                    value = machineConfig.AppSettings.Settings[appSettingName].Value;
                }
                else
                {
                    return false;
                }

                var convertedValue = attribute.Convert(value, propertyInfo.PropertyType);
                propertyInfo.SetValue(configurationInstance, convertedValue, null);
                return true;
            }
            else if (attribute is ConfigureUsingAppConfigConnectionStringAttribute)
            {
                var configureUsingAppConfigConnectionStringAttribute = attribute as ConfigureUsingAppConfigConnectionStringAttribute;
                var connectionStringName = configureUsingAppConfigConnectionStringAttribute.ConnectionStringName;
                if (string.IsNullOrWhiteSpace(connectionStringName))
                {
                    connectionStringName = propertyInfo.Name;
                }

                object value;
                if (appConfig.ConnectionStrings.ConnectionStrings.Cast<ConnectionStringSettings>().Any(css => css.Name.Equals(connectionStringName, StringComparison.OrdinalIgnoreCase)))
                {
                    value = appConfig.ConnectionStrings.ConnectionStrings.Cast<ConnectionStringSettings>().First(css => css.Name.Equals(connectionStringName, StringComparison.OrdinalIgnoreCase));
                }
                else if (machineConfig.ConnectionStrings.ConnectionStrings.Cast<ConnectionStringSettings>().Any(css => css.Name.Equals(connectionStringName, StringComparison.OrdinalIgnoreCase)))
                {
                    value = machineConfig.ConnectionStrings.ConnectionStrings.Cast<ConnectionStringSettings>().First(css => css.Name.Equals(connectionStringName, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    return false;
                }

                var convertedValue = attribute.Convert(value, propertyInfo.PropertyType);
                propertyInfo.SetValue(configurationInstance, convertedValue, null);
                return true;
            }

            

            return false;
        }
    }
}
