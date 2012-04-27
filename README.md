Softweyr.Configuration
======================

Simple configuration implementation.

<pre><code>namespace Softweyr.Configuration.Samples
{
    using Softweyr.Configuration;

    public interface IMySampleConfiguration
    {
        // This configuration property will contain the value of the key-value pair with the key
        // "SimpleConfigValueKey" specified in the App.Config / Web.Config file's appSettings section.
        [ConfigurableFromAppConfig("SimpleConfigValueKey")]
        int MyConfigurableValue { get; set; }
    }
}</code></pre>

Complex configuration implementation.

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
        [ConfigurableFromAppConfig("SimpleConfigValueKey")]
        public int MyConfigurableValue { get; set; }

        // This configuration property will try to be populated from the windows registry key if it exists,
        // if not, it will try to get the value from the AppConfigValueKey appSetting in the Web.Config
        //  / App.Config file for the current ApplicationDomain, if this is also missing then it will
        // default to "No Value Set.".
        [ConfigurableFromWindowsRegistry(1, "LOCALMACHINE|Software|Softweyr|Sample")]
        [ConfigurableFromAppConfig(2, "AppConfigKeyValue")]
        [DefaultTo("No value set.")]
        public string MyAverageConfigurationValue { get; set; }
        
        // This configuration property will try to be populated from the App.Config / Web.Config file but
        // will use a custom parser to get the value.
        // Note: Example of MyCustomClassAppConfigExtraction class is given later.
        [ConfigurableFromAppConfig(1, "AppConfigKeyValue", , typeof(MyCustomClassAppConfigExtraction))]
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
            Configure.LoadConfigurationMethodsImplicitly();
        }
    
        public void ExplicitInitialization()
        {
            // Will load all configuration methods explicitly defined in the parameter list.
            Configure
                .LoadConfigurationMethods(
                    typeof(WindowsRegistryConfigurationMethod),
                    typeof(AppConfigConfigurationMethod),
                    typeof(MyCustomDatabaseConfigurationMethod))
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
        Configure.LoadConfigurationMethodsImplicitly().RegisterConfigurationsWithNinject();
        var myDependantClass = kernal.Resolve<MyDependantClass>();
        Console.WriteLine(myDependantClass.Configuration.MyConfigurableValue);
    }
}</code></pre>