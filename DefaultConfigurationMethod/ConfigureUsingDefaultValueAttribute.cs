namespace Softweyr.Configuration
{
    using System;
    
    public class ConfigureUsingDefaultValueAttribute : ConfigureAttribute
    {
        public object DefaultValue { get; private set; }

        public ConfigureUsingDefaultValueAttribute(object defaultValue)
            : base(Precedence.Global)
        {
            this.DefaultValue = defaultValue;
        }
    }
}
