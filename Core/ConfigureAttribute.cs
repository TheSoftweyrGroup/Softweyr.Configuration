namespace Softweyr.Configuration
{
    public abstract class ConfigureAttribute : System.Attribute
    {
        protected ConfigureAttribute(Precedence precedence)
        {
            this.Precedence = precedence;
        }

        public Precedence Precedence { get; set; }
    }
}
