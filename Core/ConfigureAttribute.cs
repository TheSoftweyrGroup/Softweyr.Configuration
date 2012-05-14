using System;
using System.Reflection;

namespace Softweyr.Configuration
{
    public abstract class ConfigureAttribute : System.Attribute
    {
        protected ConfigureAttribute(Precedence precedence, Type typeConverter)
        {
            this.Precedence = precedence;
            if (typeConverter.IsSubclassOf(typeof(ITypeConverter)))
            {
                throw new ArgumentException("typeConverter must be of type ITypeConverter");
            }

            this.ConvertFunction = (ITypeConverter)Activator.CreateInstance(typeConverter);
        }

        public Precedence Precedence { get; private set; }

        private ITypeConverter ConvertFunction { get; set; }

        public object Convert(object source, Type targetType)
        {
            return this.ConvertFunction.Convert(source, targetType);
        }
    }
}
