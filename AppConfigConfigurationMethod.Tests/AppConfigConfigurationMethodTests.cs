using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Softweyr.Configuration;

namespace Softweyr.Configuration.Tests
{
    [TestFixture]
    public class AppConfigConfigurationMethodTests
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
            Configure.TheEnvironment.AddConfigurationMethod<AppConfigConfigurationMethodProvider>();
            var configuration = Configure.Get<IAppSettingConfiguration>();
            Assert.AreEqual("Hello World", configuration.TestProperty);
        }
    }

    public interface IAppSettingConfiguration
    {
        [ConfigureUsingAppConfigAppSetting]
        string TestProperty { get; set; }
    }
}
