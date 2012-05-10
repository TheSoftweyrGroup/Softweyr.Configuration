namespace Softweyr.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public interface IConfigurationMethodProvider
    {
        IEnumerable<Type> ConfigureAttributeTypesSupported { get; }
        bool TryPopulate(object configurationInstance, PropertyInfo propertyInfo, ConfigureAttribute attribute);
    }
}
