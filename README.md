Softweyr.Configuration
======================

Overview
--------

The Softweyr.Configuration component gives a convient method of adding configuration options to a
library without polluting the consuming classes with Configuration concerns.

Using the Configure class to set up the configuration environment and fetch configurations from
various data sources using configuration methods can be used to fetch configuration values from
different sources, for example the App.Config, Web.Config, windows registry, a stored procedure,
an INI file, an XML file, etc.

Examples
--------

Simple configuration by attributes implementation.

<pre><code>namespace Softweyr.Configuration.Samples
{
    using Softweyr.Configuration;

    public interface IMySampleConfiguration
    {
        // This configuration property will contain the value of the key-value pair with the key
        // "SimpleConfigValueKey" specified in the App.Config / Web.Config file's appSettings section.
        [ConfigureFromAppConfig("SimpleConfigValueKey")]
        int MyConfigurableValue { get; set; }
    }
}</code></pre>

Complex configuration by attributes implementation.

<pre><code>namespace Softweyr.Configuration.Samples
{
    using Softweyr.Configuration;

    public class MySampleConfiguration
    {
        // This configuration property will always contain the value "To Do.".
        [ConfigureUsingDefaultValue("To Do.")]
        public string MyStaticConfigurationValue { get; set; }

        // This configuration property will contain the value of the key-value pair with the key
        // "SimpleConfigValueKey" specified in the App.Config / Web.Config file's appSettings section.
        [ConfigureFromAppConfig("SimpleConfigValueKey")]
        public int MyConfigurableValue { get; set; }

        // This configuration property will try it will try to get the value from the AppConfigValueKey
        // appSetting in the Web.Config / App.Config file for the current ApplicationDomain, if not, to
        // be populated from the windows registry key if it exists, if this is also missing then it will
        // default to "No Value Set.".
        [ConfigureFromAppConfig("AppConfigKeyValue")]
        [ConfigureFromWindowsRegistry("LOCALMACHINE|Software|Softweyr|Sample")]
        [ConfigureUsingDefaultValue("No value set.")]
        public string MyAverageConfigurationValue { get; set; }
        
        // This configuration property will try to be populated from the App.Config / Web.Config file but
        // will use a custom parser to get the value.
        // Note: Example of MyCustomClassAppConfigExtraction class is given later.
        [ConfigureFromAppConfig("AppConfigKeyValue", , typeof(MyCustomClassAppConfigExtraction))]
        public MyCustomClass MyConfigurableValue { get; set; }
        
        // This configuration value will never be initialized and will simply hold the default value for
        // the property type.
        public int MyConfigurableValue { get; set; }
    }
}</code></pre>

The following examples use the above configuration class.

Initialization options and simple sample usage.

<pre><code>namespace Softweyr.Configuration.Samples
{
    using Softweyr.Configuration;

    public class Example
    {
        public void ImplicitInitialization()
        {
            // Will load all configuration methods currently referenced in the app domain.
            Configure.AddAllConfigurationMethodProviders();
        }
    
        public void ExplicitInitialization()
        {
            // Will load all configuration methods explicitly defined in the parameter list.
            Configure.AddConfigurationMethodProvider&lt;AppConfigConfigurationMethodProvider&gt;();
			Configure.AddConfigurationMethodProvider&lt;WindowsRegistry&gt;();
			Configure.AddConfigurationMethodProvider&lt;MyCustomDatabaseConfigurationMethod&gt;();
        }

		public void ImplicitFluentInitialization()
		{
			Configure
				.TheEnvironment
				.SoICanDefineAnySetting()
		}

		public void ExplicitFluentInitialization()
		{
			Configure
				.TheEnvironment
				.SoICanDefine()
				.ApplicationConfigurationFileSettings()
				.RegistryKeySettings()
				.AndSpecifyDefaultSettings()
		}
    }
}</code></pre>

Simple usage example

<pre><code>var myConfiguration = Configure.Get&lt;MySampleConfiguration&gt;();
Console.WriteLine(myConfiguration.MyConfigurableValue);</code></pre>

Commiting changes to configuration

<pre><code>var myConfiguration = Configure.Get&lt;MySampleConfiguration&gt;();
myConfiguration.MyConfigurableValue = "Hello World";
Configure.Save(myConfiguration);
</code></pre>

Example usage with Ninject

<pre><code>namespace Softweyr.Configuration.Samples
{
    using Softweyr.Configuration;
    using Ninject;
    
    public class MyDependantClass
    {
        public MySampleConfiguration configuration;
    
        public MyDependantClass(MySampleConfiguration configuration)
        {
            this.configuration = configuration;
        }
    }
    
    public class NinjectSamples
    {
        var kernel = new Kernel();
        Configure
				.TheEnvironment
				.SoICanDefineAnySetting()
				.ThenHookIntoMyNinjectKernel(kernel)
        var myDependantClass = kernel.Resolve<MyDependantClass>();
        Console.WriteLine(myDependantClass.Configuration.MyConfigurableValue);
    }
}</code></pre>

Road Map
--------

<strong>Release 1 (<i>In Progress</i>)</strong>

* Core Configuration Engine (Alpha)
    * Get<>() configuration method. (Alpha)
    * Prioritized population of configuration values. (Alpha)
* Fluent Initialization
* Explicit Initialization (Alpha)
* DefaultTo Configuration Method (In Progress)
* App.Settings Configuration Method (In Progress)
* Sample Configuration Method Project
* Basic Sample Project

<strong>Release 2</strong>

* Implicit Initialization
* Hooking mechanism for inversion of control (IOC) engines
* Ninject implementation of the IOC hooking mechanism
* Windows Registry Configuration Method
* Sample project of Implicit Initialization.
* Sample project of Ninject integration.

<strong>Release 3</strong>

* XML Configuration Method
* INI Configuration Method
* Command Line Arguments Configuration Method