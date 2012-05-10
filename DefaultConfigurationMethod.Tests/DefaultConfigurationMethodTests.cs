using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Softweyr.Configuration;

namespace Softweyr.Configuration.Tests
{
    [TestFixture]
    public class DefaultConfigurationMethodTests
    {
        [SetUp]
        public void TestSetup()
        {
            Configure.ResetTheEnvironment();
        }

        [Test]
        public void CanAddToConfigurationEnvironment()
        {
            Configure.TheEnvironment.AddConfigurationMethod<DefaultConfigurationMethodProvider>();
        }

        [Test]
        public void CanGetConfigurationWithConfigureUsingDefaultValueAttributes()
        {
            Configure.TheEnvironment.AddConfigurationMethod<DefaultConfigurationMethodProvider>();
            var config = Configure.Get<IDefaultTestConfiguration>();
            Assert.AreEqual("Hello World", config.Property1);
            Assert.AreEqual(5, config.Property2);
        }
    }

    public interface IDefaultTestConfiguration
    {
        [ConfigureUsingDefaultValue("Hello World")]
        string Property1 { get; set; }

        [ConfigureUsingDefaultValue(5)]
        int Property2 { get; set; }
    }
}
