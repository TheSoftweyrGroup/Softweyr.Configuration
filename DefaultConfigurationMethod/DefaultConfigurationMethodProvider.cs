using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Softweyr.Configuration
{
    public class DefaultConfigurationMethodProvider : IConfigurationMethodProvider
    {
        public IEnumerable<Type> ConfigureAttributeTypesSupported
        {
            get { return new[] { typeof(ConfigureUsingDefaultValueAttribute) }; }
        }

        public bool TryPopulate(object configurationInstance, System.Reflection.PropertyInfo propertyInfo, ConfigureAttribute attribute)
        {
            var configureDefaultAttribute = attribute as ConfigureUsingDefaultValueAttribute;
            propertyInfo.SetValue(configurationInstance, configureDefaultAttribute.DefaultValue, null);
            return true;
        }
    }
}
