namespace Softweyr.Configuration.Tests
{
    using Softweyr.Configuration;
    using NUnit.Framework;

    [TestFixture]
    public class ConfigureUnitTests
    {
        [SetUp]
        public void TestSetUp()
        {
            Configure.ResetTheEnvironment();
        }

        [Test]
        public void GetConfigurationBeforeLoadingConfigurationMethods()
        {
            // Arrange
            // Action
            var testCase = new TestDelegate(() => Configure.Get<IMyTestConfiguration>());

            // Assert
            Assert.Throws(typeof(ConfigurationEnvironmentNotInitializedException), testCase);
        }

        [Test]
        public void ConfigureExplicitlyWithValidConfigurationProviderType()
        {
            // Arrange

            // Action
            Configure.TheEnvironment.AddConfigurationMethod<GoodMockConfigurationMethodProvider>();

            // Assert
        }

        [Test]
        public void GetCofnigurationAfterLoadingConfigurationMethods()
        {
            // Arrange
            Configure.TheEnvironment.AddConfigurationMethod<GoodMockConfigurationMethodProvider>();
            TestDelegate testDelegate = () => Configure.Get<IMyTestConfiguration>();

            // Action
            Assert.DoesNotThrow(testDelegate);
        }

        [Test]
        public void GetAConfigurationFromPlainOldCSharpInterface()
        {
            // Arrange
            Configure.TheEnvironment.AddConfigurationMethod<GoodMockConfigurationMethodProvider>();

            // Action
            var configuration = Configure.Get<IMyTestConfiguration>();

            // Assert
            Assert.IsNotNull(configuration);
            Assert.IsNull(configuration.TestValue1);
        }
    }

    public interface IMyTestConfiguration
    {
        string TestValue1 { get; set; }
    }

    public class BadMockConfigurationMethodProvider
    {
    }

    public class GoodMockConfigurationMethodProvider : IConfigurationMethodProvider
    {
    }
}
