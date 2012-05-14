using System;
using System.Reflection;

namespace Softweyr.Configuration
{
    public abstract class ConfigureAttribute : System.Attribute
    {
        private static object DefaultConvertFunction(object sourceValue, Type targetType)
        {
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(targetType);
            return converter.ConvertFrom(sourceValue);
        }

        protected ConfigureAttribute(Precedence precedence)
        {
            this.Precedence = precedence;
            this.ConvertFunction = DefaultConvertFunction;
        }

        protected ConfigureAttribute(Precedence precedence, TypeConverterDelegate convertFunction)
        {
            this.Precedence = precedence;
            this.ConvertFunction = convertFunction;
        }

        public Precedence Precedence { get; private set; }

        public TypeConverterDelegate ConvertFunction { get; private set; }

        public object Convert(object source, Type targetType)
        {
            return this.ConvertFunction.Invoke(source, targetType);
        }
    }
}
