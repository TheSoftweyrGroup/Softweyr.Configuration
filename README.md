Softweyr.Configuration
======================

Sample Usage

<pre>

public class MySampleConfiguration
{
    [ConfigurableFromWindowsRegistry(1, "LOCALMACHINE|Software|Softweyr|Sample")]
    [ConfigurableFromWindowsRegistry(2, "LOCALMACHINE|Software|Softweyr|Sample2")]
    [ConfigurableFromAppConfig(3, "AppConfigKeyValue")]
    [DefaultTo("No value set.")]
    public string MyConfigurableValue { get; set; }
}

</pre>

<pre>
using Softweyr.Configuration;

public class Example
{
    public void Initialize()
    {
        Configure.LoadConfigurationMethodsImplicitly().AutoRegisterConfigurationsWithNinject();
    }
    
    public void UsageSampleOne()
    {
        Configure.Get<MySampleConfiguration>()
    }
}


</pre>