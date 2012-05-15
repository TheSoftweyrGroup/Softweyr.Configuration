using System; 
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Text;
using NUnit.Framework;
using Softweyr.Configuration;

namespace Softweyr.Configuration.Tests
{
    public class AppConfigBuilder
    {
        private const string ConfigFileTemplate = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
  <appSettings>
    {0}
  </appSettings>
  <connectionStrings>
    {1}
  </connectionStrings>
</configuration>";

        private const string AppSettingTemplate = @"<add key=""{0}"" value=""{1}"" />";
        private const string ConnectionStringTemplate = @"<add name=""{0}"" providerName=""{1}"" connectionString=""{2}"" />";

        private readonly Dictionary<string, string> appSettings = new Dictionary<string, string>();
        private readonly List<System.Configuration.ConnectionStringSettings> connectionStrings = new List<ConnectionStringSettings>();

        public void AddAppSetting(string key, string value)
        {
            this.appSettings.Add(key, value);
        }

        public void AddConnectionString(string name, string provider, string connectionString)
        {
            this.connectionStrings.Add(new ConnectionStringSettings(name, connectionString, provider));
        }

        public override string ToString()
        {
            var appSettingsString = string.Join(Environment.NewLine,
                                                appSettings.Select(
                                                    kvp => string.Format(AppSettingTemplate, kvp.Key, kvp.Value)));

            var connectionStringSettings = string.Join(Environment.NewLine,
                                                connectionStrings.Select(
                                                    connString => string.Format(ConnectionStringTemplate, connString.Name, connString.ProviderName, connString.ConnectionString)));

            return string.Format(ConfigFileTemplate, appSettingsString, connectionStringSettings);
        }
    }

    [TestFixture]
    public class AppConfigConfigurationMethodTests : MarshalByRefObject
    {
        [SetUp]
        public void Setup()
        {
            Configure.ResetTheEnvironment();
        }

        [Test]
        public void CanAddToTheConfigureEnvironment()
        {
            Configure.TheEnvironment.AddConfigurationMethod<AppConfigConfigurationMethodProvider>();
        }

        [Test]
        public void CanGetAppSetting()
        {
            var tempConfigurationFile = System.IO.Path.GetTempFileName();
            try
            {
                var builder = new AppConfigBuilder();
                builder.AddAppSetting("TestProperty", "Hello World");
                System.IO.File.WriteAllText(tempConfigurationFile, builder.ToString());
                var baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace("file:\\",
                                                                                                            string.Empty);
                var appDomain = AppDomain.CreateDomain("CanGetAppSetting", null,
                                                       new AppDomainSetup()
                                                           {
                                                               ConfigurationFile = tempConfigurationFile,
                                                               PrivateBinPath = baseDirectory,
                                                               ApplicationBase = baseDirectory
                                                           });
                var worker = (AppConfigConfigurationMethodTests) appDomain.CreateInstanceAndUnwrap(
                    Assembly.GetExecutingAssembly().FullName,
                    typeof (AppConfigConfigurationMethodTests).FullName);

                IAppSettingConfiguration configuration = null;
                configuration = (IAppSettingConfiguration)worker.Execute(() =>
                                                   {
                                                       Configure.TheEnvironment.AddConfigurationMethod
                                                           <AppConfigConfigurationMethodProvider>();
                                                       return Configure.Get<IAppSettingConfiguration>();
                                                   });

                Assert.AreEqual("Hello World", configuration.TestProperty);
            }
            finally
            {
                File.Delete(tempConfigurationFile);
            }
        }

        public object Execute(Func<object> function)
        {
            return function.Invoke();
        }
    }

    public interface IAppSettingConfiguration
    {
        [ConfigureUsingAppConfigAppSetting]
        string TestProperty { get; set; }
    }
}
