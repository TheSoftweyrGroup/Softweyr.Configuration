using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Softweyr.Configuration
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class ConfigureUsingAppConfigConnectionStringAttribute : ConfigureAttribute
    {
        public string ConnectionStringName { get; private set; }

        public ConfigureUsingAppConfigConnectionStringAttribute()
            : this(Precedence.Application, typeof(DefaultTypeConverter))
        {
        }

        public ConfigureUsingAppConfigConnectionStringAttribute(string connectionStringName)
            : this(Precedence.Application, typeof(DefaultTypeConverter), connectionStringName)
        {
        }

        public ConfigureUsingAppConfigConnectionStringAttribute(Precedence precedence)
            : this(precedence, typeof(DefaultTypeConverter))
        {
        }

        public ConfigureUsingAppConfigConnectionStringAttribute(Precedence precedence, string connectionStringName)
            : this(precedence, typeof(DefaultTypeConverter), connectionStringName)
        {
        }

        public ConfigureUsingAppConfigConnectionStringAttribute(Type typeConverter)
            : this(Precedence.Application, typeConverter)
        {
        }

        public ConfigureUsingAppConfigConnectionStringAttribute(Type typeConverter, string connectionStringName)
            : this(Precedence.Application, typeConverter, connectionStringName)
        {
        }

        public ConfigureUsingAppConfigConnectionStringAttribute(Precedence precedence, Type typeConverter)
            : base(precedence, typeConverter)
        {
            this.ConnectionStringName = null;
        }

        public ConfigureUsingAppConfigConnectionStringAttribute(Precedence precedence, Type typeConverter, string appSettingName)
            : base(precedence, typeConverter)
        {
            if (string.IsNullOrWhiteSpace(appSettingName))
            {
                throw new AppSettingKeyCannotBeNullOrWhitespaceException();
            }

            this.ConnectionStringName = appSettingName.Trim();
        }
    }
}
