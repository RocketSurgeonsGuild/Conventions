namespace Rocket.Surgery.Conventions
{
    public class RocketEnvironment : IRocketEnvironment
    {
        public RocketEnvironment(string environmentName, string applicationName)
        {
            EnvironmentName = environmentName;
            ApplicationName = applicationName;
        }

        public string EnvironmentName { get; }
        public string ApplicationName { get; }
    }
}
