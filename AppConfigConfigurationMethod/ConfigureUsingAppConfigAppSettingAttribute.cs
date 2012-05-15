using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Softweyr.Configuration
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class ConfigureUsingAppConfigAppSettingAttribute : ConfigureAttribute
    {
        public string AppSettingName { get; private set; }

        public ConfigureUsingAppConfigAppSettingAttribute()
            : this(Precedence.Application, typeof(DefaultTypeConverter))
        {
        }

        public ConfigureUsingAppConfigAppSettingAttribute(string appSettingName)
            : this(Precedence.Application, typeof(DefaultTypeConverter), appSettingName)
        {
        }

        public ConfigureUsingAppConfigAppSettingAttribute(Precedence precedence)
            : this(precedence, typeof(DefaultTypeConverter))
        {
        }

        public ConfigureUsingAppConfigAppSettingAttribute(Precedence precedence, string appSettingName)
            : this(precedence, typeof(DefaultTypeConverter), appSettingName)
        {
        }

        public ConfigureUsingAppConfigAppSettingAttribute(Type typeConverter)
            : this(Precedence.Application, typeConverter)
        {
        }

        public ConfigureUsingAppConfigAppSettingAttribute(Type typeConverter, string appSettingName)
            : this(Precedence.Application, typeConverter, appSettingName)
        {
        }

        public ConfigureUsingAppConfigAppSettingAttribute(Precedence precedence, Type typeConverter)
            : base(precedence, typeConverter)
        {
            this.AppSettingName = null;
        }

        public ConfigureUsingAppConfigAppSettingAttribute(Precedence precedence, Type typeConverter, string appSettingName)
            : base(precedence, typeConverter)
        {
            if (string.IsNullOrWhiteSpace(appSettingName))
            {
                throw new AppSettingKeyCannotBeNullOrWhitespaceException();
            }

            this.AppSettingName = appSettingName.Trim();
        }
    }
}
