namespace Softweyr.Configuration
{
    using System;

    public sealed class Configure
    {
        private static Configure environment = new Configure();

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

        public Configure ByLoadingConfigurationMethods(params IConfigurationMethod[] configurationMethods)
        {
            // To Do: Add the specified configuration methods to the environment configure instance.
            throw new NotImplementedException();
        }

        public TConfiguration Get<TConfiguration>()
        {
            // To do: Return an instance of the given configuration.
            throw new NotImplementedException();
        }

        public static TConfiguration Get<TConfiguration>()
        {
            return environment.Get<TConfiguration>();
        }
    }
}
