namespace Softweyr.Configuration.Tests
{
    using System;
    using System.Collections.Generic;
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
        public void ConfigureExplicitlyWithMultipleValidConfigurationProviderType()
        {
            // Arrange
            Configure.TheEnvironment.AddConfigurationMethod<GoodMockConfigurationMethodProvider>();

            // Action
            var testDelegate = new TestDelegate(() =>
            {
                Configure.TheEnvironment.AddConfigurationMethod<GoodMockConfigurationMethodProvider2>();
                Configure.TheEnvironment.AddConfigurationMethod<GoodMockConfigurationMethodProvider3>();
            });

            // Assert
            Assert.DoesNotThrow(testDelegate);
        }

        [Test]
        public void ConfigureExplicitlyWithMultipleValidConfigurationProvidersAndOneInvalidProvider()
        {
            // Arrange
            Configure.TheEnvironment.AddConfigurationMethod<GoodMockConfigurationMethodProvider>();
            Configure.TheEnvironment.AddConfigurationMethod<GoodMockConfigurationMethodProvider2>();

            // Action
            var testDelegate = new TestDelegate(() =>
                {
                    Configure.TheEnvironment.AddConfigurationMethod<BadMockConfigurationMethodProvider2>();
                });

            // Assert
            Assert.Throws<ProviderMustSupportAtLeastOneConfigureAttributeType>(testDelegate);
        }

        [Test]
        public void GetConfigurationAfterLoadingConfigurationMethods()
        {
            // Arrange
            // By Initializing the environment.
            Configure.TheEnvironment.AddConfigurationMethod<GoodMockConfigurationMethodProvider>();
            
            // Action & Assert
            TestDelegate testDelegate = () => Configure.Get<IMyTestConfiguration>();
            Assert.DoesNotThrow(testDelegate);
        }

        [Test]
        public void GetAConfigurationFromPlainOldCSharpInterface()
        {
            // Arrange
            // By Initializing the environment.
            Configure.TheEnvironment.AddConfigurationMethod<GoodMockConfigurationMethodProvider>();

            // Action
            var configuration = Configure.Get<IMyTestConfiguration>();

            // Assert
            Assert.IsNotNull(configuration);
            Assert.IsInstanceOf<IMyTestConfiguration>(configuration);
            Assert.IsNull(configuration.TestValue1);
        }

        [Test]
        public void GetAConfigurationFromPlainOldCSharpObject()
        {
            // Arrange
            // By Initializing the environment.
            Configure.TheEnvironment.AddConfigurationMethod<GoodMockConfigurationMethodProvider>();

            // Action
            var configuration = Configure.Get<MyTestConfiguration>();

            // Assert
            Assert.IsNotNull(configuration);
            Assert.IsInstanceOf<MyTestConfiguration>(configuration);
            Assert.IsNull(configuration.TestValue2);
        }

        [Test]
        public void ProviderDoesNotSupportsNonConfigureAttributeType()
        {
            // Arrange
            // By Initializing the environment.
            var configurationProvider = new GoodMockConfigurationMethodProvider();
            configurationProvider.ConfigureAttributeTypesSupported = new[] { typeof(ConfigureBadMockAttribute) };
            
            // Action & Assert
            TestDelegate testDelegate = () => Configure.TheEnvironment.AddConfigurationMethod(configurationProvider); ;
            Assert.Throws<UnsupportedAttributeTypeException>(testDelegate);
        }

        [Test]
        public void ProviderMustSupportAtLeastOneConfigureAttributeType()
        {
            // Arrange
            // By Initializing the environment.
            var configurationProvider = new GoodMockConfigurationMethodProvider();
            configurationProvider.ConfigureAttributeTypesSupported = new Type[] { };

            // Action & Assert
            TestDelegate testDelegate = () => Configure.TheEnvironment.AddConfigurationMethod(configurationProvider); ;
            Assert.Throws<ProviderMustSupportAtLeastOneConfigureAttributeType>(testDelegate);
        }

        [Test]
        public void ProviderSupportConfigureAttributeType()
        {
            // Arrange
            // By Initializing the environment.
            var configurationProvider = new GoodMockConfigurationMethodProvider();
            configurationProvider.ConfigureAttributeTypesSupported = new[] { typeof(ConfigureGoodMockAttribute) };

            // Action & Assert
            TestDelegate testDelegate = () => Configure.TheEnvironment.AddConfigurationMethod(configurationProvider); ;
            Assert.DoesNotThrow(testDelegate);
        }

        [Test]
        public void CorrectValuesArePassedToProvider()
        {
            // Arrange
            // By Initializing the environment.
            var configurationProvider = new GoodMockConfigurationMethodProvider();
            Configure.TheEnvironment.AddConfigurationMethod(configurationProvider);
            configurationProvider.TryPopulateBehaviour =
                (configurationInstance, propertyInfo, attribute) =>
                {
                    Assert.IsNotNull(configurationInstance);
                    Assert.IsInstanceOf<IMyTestConfiguration2>(configurationInstance);
                    Assert.IsNotNull(propertyInfo);
                    Assert.AreEqual("TestValue3", propertyInfo.Name);
                    Assert.AreEqual(typeof(IMyTestConfiguration2), propertyInfo.DeclaringType);
                    Assert.IsNotNull(attribute);
                    Assert.IsInstanceOf<ConfigureGoodMockAttribute>(attribute);
                    Assert.Pass();
                    return true;
                };

            // Action
            var configuration = Configure.Get<IMyTestConfiguration2>();

            // Assert
            Assert.Fail();
        }

        [Test]
        public void CorrectProviderIsCalledDependingOnConfigureAttributeType()
        {
            // Arrange
            var provider1 = new GoodMockConfigurationMethodProvider();
            var provider2 = new GoodMockConfigurationMethodProvider2();
            Configure.TheEnvironment.AddConfigurationMethod(provider1);
            Configure.TheEnvironment.AddConfigurationMethod(provider2);
            int provider1CallCount = 0;
            int provider2CallCount = 0;
            provider1.TryPopulateBehaviour = (configurationInstance, propertyInfo, attribute) => { provider1CallCount++; return true; };
            provider2.TryPopulateBehaviour = (configurationInstance, propertyInfo, attribute) => { provider2CallCount++; return true; };

            // Action
            provider1CallCount = 0;
            provider2CallCount = 0;
            var configuration1 = Configure.Get<IMyTestConfiguration>(); // No Provider to be called.
            Assert.AreEqual(0, provider1CallCount);
            Assert.AreEqual(0, provider2CallCount);

            provider1CallCount = 0;
            provider2CallCount = 0;
            var configuration2 = Configure.Get<IMyTestConfiguration2>(); // Expect provider 2 to be called.
            Assert.AreEqual(1, provider1CallCount);
            Assert.AreEqual(0, provider2CallCount);

            provider1CallCount = 0;
            provider2CallCount = 0;
            var configuration3 = Configure.Get<IMyTestConfiguration3>(); // Expect both to be called
            Assert.AreEqual(1, provider1CallCount);
            Assert.AreEqual(1, provider2CallCount);
        }

        [Test]
        public void ProvidersAreCheckedDependingOnTheConfigureAttributePrecedence()
        {
            // Arrange
            var provider1 = new GoodMockConfigurationMethodProvider();
            var provider2 = new GoodMockConfigurationMethodProvider2();
            provider1.ConfigureAttributeTypesSupported = new Type[] { typeof(HighPrecedenceConfigureAttribute) };
            provider2.ConfigureAttributeTypesSupported = new Type[] { typeof(LowPrecedenceConfigureAttribute) };
            Configure.TheEnvironment.AddConfigurationMethod(provider1);
            Configure.TheEnvironment.AddConfigurationMethod(provider2);
            int provider1Step = 0;
            int provider2Step = 0;
            int callCount = 0;
            provider1.TryPopulateBehaviour = (configurationInstance, propertyInfo, attribute) => { if (provider1Step != 0) { Assert.Fail(); } provider1Step = ++callCount; return false; };
            provider2.TryPopulateBehaviour = (configurationInstance, propertyInfo, attribute) => { if (provider2Step != 0) { Assert.Fail(); } provider2Step = ++callCount; return false; };

            // Action
            var configuration1 = Configure.Get<IPrecedenceTestConfiguration>(); // No Provider to be called.
            Assert.AreEqual(2, callCount);
            Assert.AreEqual(1, provider1Step);
            Assert.AreEqual(2, provider2Step);
        }
    }

    public interface IMyTestConfiguration
    {
        string TestValue1 { get; set; }
    }

    public class MyTestConfiguration
    {
        public string TestValue2 { get; set; }
    }

    public interface IMyTestConfiguration2
    {
        [ConfigureGoodMockAttribute("Test Value 3")]
        string TestValue3 { get; set; }
    }

    public interface IMyTestConfiguration3
    {
        [ConfigureGoodMock2("Test Value 4")]
        string TestValue4 { get; set; }

        [ConfigureGoodMock("Test Value 5")]
        string TestValue5 { get; set; }
    }

    public interface IPrecedenceTestConfiguration
    {
        [HighPrecedenceConfigure("Test Value 4")]
        [LowPrecedenceConfigure("Test Value 4")]
        string TestValue6 { get; set; }
    }
    

    public class BadMockConfigurationMethodProvider
    {
    }

    public class BadMockConfigurationMethodProvider2 : IConfigurationMethodProvider
    {
        public BadMockConfigurationMethodProvider2()
        {
            this.TryPopulateBehaviour = (configurationInstance, propertyInfo, attribute) => true;
            this.ConfigureAttributeTypesSupported = new Type[] { };
        }

        public IEnumerable<Type> ConfigureAttributeTypesSupported { get; set; }

        public System.Func<object, System.Reflection.PropertyInfo, ConfigureAttribute, bool> TryPopulateBehaviour { get; set; }

        public bool TryPopulate(object configurationInstance, System.Reflection.PropertyInfo propertyInfo, ConfigureAttribute attribute)
        {
            return TryPopulateBehaviour.Invoke(configurationInstance, propertyInfo, attribute);
        }
    }

    public class GoodMockConfigurationMethodProvider : IConfigurationMethodProvider
    {
        public GoodMockConfigurationMethodProvider()
        {
            this.TryPopulateBehaviour = (configurationInstance, propertyInfo, attribute) => true;
            this.ConfigureAttributeTypesSupported = new[] { typeof(ConfigureGoodMockAttribute) };
        }

        public IEnumerable<Type> ConfigureAttributeTypesSupported { get; set; }

        public System.Func<object, System.Reflection.PropertyInfo, ConfigureAttribute, bool> TryPopulateBehaviour { get; set; }

        public bool TryPopulate(object configurationInstance, System.Reflection.PropertyInfo propertyInfo, ConfigureAttribute attribute)
        {
            return TryPopulateBehaviour.Invoke(configurationInstance, propertyInfo, attribute);
        }
    }

    public class GoodMockConfigurationMethodProvider2 : IConfigurationMethodProvider
    {
        public GoodMockConfigurationMethodProvider2()
        {
            this.TryPopulateBehaviour = (configurationInstance, propertyInfo, attribute) => true;
            this.ConfigureAttributeTypesSupported = new[] { typeof(ConfigureGoodMock2Attribute) };
        }

        public IEnumerable<Type> ConfigureAttributeTypesSupported { get; set; }

        public System.Func<object, System.Reflection.PropertyInfo, ConfigureAttribute, bool> TryPopulateBehaviour { get; set; }

        public bool TryPopulate(object configurationInstance, System.Reflection.PropertyInfo propertyInfo, ConfigureAttribute attribute)
        {
            return TryPopulateBehaviour.Invoke(configurationInstance, propertyInfo, attribute);
        }
    }

    public class GoodMockConfigurationMethodProvider3 : IConfigurationMethodProvider
    {
        public GoodMockConfigurationMethodProvider3()
        {
            this.TryPopulateBehaviour = (configurationInstance, propertyInfo, attribute) => true;
            this.ConfigureAttributeTypesSupported = new[] { typeof(ConfigureGoodMock3Attribute) };
        }

        public IEnumerable<Type> ConfigureAttributeTypesSupported { get; set; }

        public System.Func<object, System.Reflection.PropertyInfo, ConfigureAttribute, bool> TryPopulateBehaviour { get; set; }

        public bool TryPopulate(object configurationInstance, System.Reflection.PropertyInfo propertyInfo, ConfigureAttribute attribute)
        {
            return TryPopulateBehaviour.Invoke(configurationInstance, propertyInfo, attribute);
        }
    }

    public class ConfigureGoodMockAttribute : ConfigureAttribute
    {
        public readonly string Value;

        public ConfigureGoodMockAttribute(string value)
            : base(Precedence.Medium)
        {
            this.Value = value;
        }
    }

    public class ConfigureGoodMock2Attribute : ConfigureAttribute
    {
        public readonly string Value;

        public ConfigureGoodMock2Attribute(string value)
            : base(Precedence.Medium)
        {
            this.Value = value;
        }
    }

    public class ConfigureGoodMock3Attribute : ConfigureAttribute
    {
        public readonly string Value;

        public ConfigureGoodMock3Attribute(string value)
            : base(Precedence.Medium)
        {
            this.Value = value;
        }
    }

    public class ConfigureBadMockAttribute : System.Attribute
    {
        public readonly string Value;

        public ConfigureBadMockAttribute(string value)
        {
            this.Value = value;
        }
    }

    public class HighPrecedenceConfigureAttribute : ConfigureAttribute
    {
        public readonly string Value;

        public HighPrecedenceConfigureAttribute(string value)
            : base(Precedence.High)
        {
            this.Value = value;
        }
    }

    public class LowPrecedenceConfigureAttribute : ConfigureAttribute
    {
        public readonly string Value;

        public LowPrecedenceConfigureAttribute(string value)
            : base(Precedence.Low)
        {
            this.Value = value;
        }
    }
}
