namespace Softweyr.Configuration
{
    using System;
    
    public class ConfigureUsingDefaultValueAttribute : ConfigureAttribute
    {
        public object DefaultValue { get; private set; }

        public ConfigureUsingDefaultValueAttribute(object defaultValue)
            : this(Precedence.Global, typeof(DefaultTypeConverter), defaultValue)
        {
        }

        public ConfigureUsingDefaultValueAttribute(Precedence precedence, object defaultValue)
            : this(precedence, typeof(DefaultTypeConverter), defaultValue)
        {
        }

        public ConfigureUsingDefaultValueAttribute(Type typeConverter, object defaultValue)
            : this(Precedence.Global, typeConverter, defaultValue)
        {
        }

        public ConfigureUsingDefaultValueAttribute(Precedence precedence, Type typeConverter, object defaultValue)
            : base(precedence, typeConverter)
        {
            this.DefaultValue = defaultValue;
        }
    }
}
