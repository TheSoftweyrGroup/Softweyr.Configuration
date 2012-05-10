using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Softweyr.Configuration
{
    public class ConfigureUsingAppConfigAppSettingAttribute : ConfigureAttribute
    {
        public string AppSettingName { get; private set; }

        public ConfigureUsingAppConfigAppSettingAttribute()
            : base(Precedence.Application)
        {
            this.AppSettingName = null;
        }

        public ConfigureUsingAppConfigAppSettingAttribute(string appSettingName)
            : base(Precedence.Application)
        {
            if (string.IsNullOrWhiteSpace(appSettingName))
            {
                // TODO: Make this a more descriptive exception
                throw new Exception("appSettingName cannot be null, empty or whitespace.");
            }

            this.AppSettingName = appSettingName.Trim();
        }
    }
}
