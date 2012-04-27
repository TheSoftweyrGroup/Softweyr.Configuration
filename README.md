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
        [DefaultTo("To Do.")]
        public string MyStaticConfigurationValue { get; set; }

        // This configuration property will contain the value of the key-value pair with the key
        // "SimpleConfigValueKey" specified in the App.Config / Web.Config file's appSettings section.
        [ConfigureFromAppConfig("SimpleConfigValueKey")]
        public int MyConfigurableValue { get; set; }

        // This configuration property will try to be populated from the windows registry key if it exists,
        // if not, it will try to get the value from the AppConfigValueKey appSetting in the Web.Config
        //  / App.Config file for the current ApplicationDomain, if this is also missing then it will
        // default to "No Value Set.".
        [ConfigureFromWindowsRegistry(1, "LOCALMACHINE|Software|Softweyr|Sample")]
        [ConfigureFromAppConfig(2, "AppConfigKeyValue")]
        [DefaultTo("No value set.")]
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
            Configure.TheEnvironment().ByLoadingConfigurationMethodsImplicitly();
        }
    
        public void ExplicitInitialization()
        {
            // Will load all configuration methods explicitly defined in the parameter list.
            Configure.TheEnvironment()
                .ByLoadingConfigurationMethods(
                    typeof(WindowsRegistryConfigurationMethod),
                    typeof(AppConfigConfigurationMethod),
                    typeof(MyCustomDatabaseConfigurationMethod));
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
        Configure.TheEnvironment()
            .ByLoadingConfigurationMethodsImplicitly()
            .ThenRegisteringConfigurationsWithNinject(kernel);
        var myDependantClass = kernel.Resolve<MyDependantClass>();
        Console.WriteLine(myDependantClass.Configuration.MyConfigurableValue);
    }
}</code></pre>