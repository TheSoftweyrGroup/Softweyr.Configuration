namespace Softweyr.Configuration
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Collections.Concurrent;

    public sealed class Configure
    {
        private static Configure environment = new Configure();

        private bool initialized = false;

        private Configure()
        {
        }

        public static void ResetTheEnvironment()
        {
            environment = new Configure();
        }

        public static Configure TheEnvironment { get { return environment; } }

        public Configure ByLoadingConfigurationMethodsImplicitly()
        {
            // To Do: search all classes loaded in the appdomain that implement IConfigurationMethod.
            throw new NotImplementedException();
        }

        public void AddConfigurationMethod<TConfigurationMethod>()
            where TConfigurationMethod : IConfigurationMethodProvider, new()
        {
            this.AddConfigurationMethod(new TConfigurationMethod());
        }

        private ConcurrentDictionary<Type, IConfigurationMethodProvider> methodProviders = new ConcurrentDictionary<Type,IConfigurationMethodProvider>();

        public void AddConfigurationMethod(IConfigurationMethodProvider configurationMethod)
        {
            this.initialized = true;
            if (configurationMethod.ConfigureAttributeTypesSupported.Count() == 0)
            {
                throw new ProviderMustSupportAtLeastOneConfigureAttributeType();
            }

            if (configurationMethod.ConfigureAttributeTypesSupported.Any(type => !type.IsSubclassOf(typeof(ConfigureAttribute))))
            {
                throw new UnsupportedAttributeTypeException();
            }

            foreach (var attributeType in configurationMethod.ConfigureAttributeTypesSupported)
            {
                if (!this.methodProviders.TryAdd(attributeType, configurationMethod))
                {
                    // TODO: Write unit test for checking if there is already a provider for this configuration method.
                    throw new NotImplementedException();
                }
            }
        }

        public TConfiguration GetConfiguration<TConfiguration>()
        {
            if (!this.initialized)
            {
                throw new ConfigurationEnvironmentNotInitializedException();
            }

            TConfiguration configurationInstance = this.GetConfigurationInstance<TConfiguration>();
            foreach (var propertyInfo in typeof(TConfiguration).GetProperties())
            {
                var configurationAttributes = propertyInfo.GetCustomAttributes(true).Where(attribute => attribute is ConfigureAttribute).Cast<ConfigureAttribute>();
                foreach (var configurationAttribute in configurationAttributes.OrderByDescending(attribute => attribute.Precedence))
                {
                    var attributeType = configurationAttribute.GetType();
                    IConfigurationMethodProvider provider;
                    
                    if (!methodProviders.TryGetValue(attributeType, out provider))
                    {
                        // TODO: Write unit test for when there is no method provider for a ConfigureAttribute.
                        throw new NotImplementedException();
                    }

                    if (provider.TryPopulate(configurationInstance, propertyInfo, configurationAttribute))
                    {
                        break;
                    }
                }
            }

            return configurationInstance;
        }

        private TConfiguration GetConfigurationInstance<TConfiguration>()
        {
            if (typeof(TConfiguration).IsInterface)
            {
                var dynamicAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new System.Reflection.AssemblyName(Guid.NewGuid().ToString()), System.Reflection.Emit.AssemblyBuilderAccess.RunAndSave);
                var dynamicModule = dynamicAssembly.DefineDynamicModule(Guid.NewGuid().ToString());
                var dynamicType = dynamicModule.DefineType(Guid.NewGuid().ToString(), TypeAttributes.Public);
                dynamicType.AddInterfaceImplementation(typeof(TConfiguration));
                foreach (var propertyInfo in typeof(TConfiguration).GetProperties())
                {
                    var fieldName = propertyInfo.Name + Guid.NewGuid().ToString();

                    var dynamicField = dynamicType.DefineField(fieldName, propertyInfo.PropertyType, FieldAttributes.Private);
                    var dynamicProperty = dynamicType.DefineProperty(propertyInfo.Name, PropertyAttributes.HasDefault, propertyInfo.PropertyType, null);

                    var dynamicPropertyGetter =
                        dynamicType.DefineMethod(
                            "get_" + propertyInfo.Name,
                            MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                            propertyInfo.PropertyType,
                            Type.EmptyTypes);
                    var getterIl = dynamicPropertyGetter.GetILGenerator();
                    getterIl.Emit(OpCodes.Ldarg_0);
                    getterIl.Emit(OpCodes.Ldfld, dynamicField);
                    getterIl.Emit(OpCodes.Ret);
                    dynamicProperty.SetGetMethod(dynamicPropertyGetter);

                    var dynamicPropertySetter =
                        dynamicType.DefineMethod(
                            "set_" + propertyInfo.Name,
                            MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                            null,
                            new Type[]
                        {
                            propertyInfo.PropertyType
                        });
                    var setterIl = dynamicPropertySetter.GetILGenerator();
                    setterIl.Emit(OpCodes.Ldarg_0);
                    setterIl.Emit(OpCodes.Ldarg_1);
                    setterIl.Emit(OpCodes.Stfld, dynamicField);
                    setterIl.Emit(OpCodes.Ret);
                    dynamicProperty.SetSetMethod(dynamicPropertySetter);
                }

                var type = dynamicType.CreateType();
                return (TConfiguration)Activator.CreateInstance(type);
            }
            else
            {
                return (TConfiguration)Activator.CreateInstance(typeof(TConfiguration));
            }
        }

        public static TConfiguration Get<TConfiguration>()
        {
            return environment.GetConfiguration<TConfiguration>();
        }
    }
}
