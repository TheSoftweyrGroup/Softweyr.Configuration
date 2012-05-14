using System; 
using System.Collections.Generic;
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
    [TestFixture]
    public class AppConfigConfigurationMethodTests : MarshalByRefObject
    {
        private const string SingleAppSettingConfigFile = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
  <appSettings>
    <add key=""{0}"" value=""{1}"" />
  </appSettings>
</configuration>";

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
                System.IO.File.WriteAllText(tempConfigurationFile,
                                            string.Format(SingleAppSettingConfigFile, "TestProperty", "Hello World"));
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
