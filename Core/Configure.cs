namespace Softweyr.Configuration
{
    using System;
    using System.Linq;

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

        public Configure AddConfigurationMethod<TConfigurationMethod>()
            where TConfigurationMethod : IConfigurationMethodProvider, new()
        {
            return this.AddConfigurationMethod(new TConfigurationMethod());
        }

        public Configure AddConfigurationMethod(IConfigurationMethodProvider configurationMethods)
        {
            this.initialized = true;
            // To Do: Add the specified configuration methods to the environment configure instance.
            return null;
        }

        public TConfiguration GetConfiguration<TConfiguration>()
        {
            if (!this.initialized)
            {
                throw new ConfigurationEnvironmentNotInitializedException();
            }

            var dynamicAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new System.Reflection.AssemblyName(Guid.NewGuid().ToString()), System.Reflection.Emit.AssemblyBuilderAccess.RunAndSave);
            var dynamicModule = dynamicAssembly.DefineDynamicModule(Guid.NewGuid().ToString());

            // To do: Return an instance of the given configuration.
            return default(TConfiguration);
        }

        public static TConfiguration Get<TConfiguration>()
        {
            return environment.GetConfiguration<TConfiguration>();
        }
    }
}
