using System;

namespace Softweyr.Configuration
{
    public class DefaultTypeConverter : ITypeConverter 
    {
        public object Convert(object sourceValue, Type targetType)
        {
            if (targetType == sourceValue.GetType())
            {
                return sourceValue;
            }

            var converter = System.ComponentModel.TypeDescriptor.GetConverter(targetType);
            return converter.ConvertFrom(sourceValue);
        }
    }

    public interface ITypeConverter
    {
        object Convert(object sourceValue, Type targetType);
    }
}