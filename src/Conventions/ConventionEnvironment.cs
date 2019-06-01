namespace Rocket.Surgery.Conventions
{
    public class ConventionEnvironment : IConventionEnvironment
    {
        public ConventionEnvironment(string environmentName, string applicationName)
        {
            EnvironmentName = environmentName;
            ApplicationName = applicationName;
        }

        public string EnvironmentName { get; }
        public string ApplicationName { get; }
    }
}
