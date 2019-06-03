using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Rocket.Surgery.Conventions
{
    public class RocketEnvironment : IRocketEnvironment
    {
        public RocketEnvironment(string environmentName, string applicationName, string contentRootPath, IFileProvider contentRootFileProvider)
        {
            EnvironmentName = environmentName;
            ApplicationName = applicationName;
            ContentRootPath = contentRootPath;
            ContentRootFileProvider = contentRootFileProvider;
        }
        public RocketEnvironment(IHostingEnvironment environment)
        {
            EnvironmentName = environment.EnvironmentName;
            ApplicationName = environment.ApplicationName;
            ContentRootPath = environment.ContentRootPath;
            ContentRootFileProvider = environment.ContentRootFileProvider;
        }
#if NETCOREAPP3_0
        public RocketEnvironment(IHostEnvironment environment)
        {
            EnvironmentName = environment.EnvironmentName;
            ApplicationName = environment.ApplicationName;
            ContentRootPath = environment.ContentRootPath;
            ContentRootFileProvider = environment.ContentRootFileProvider;
        }
#endif

        public string EnvironmentName { get; }
        public string ApplicationName { get; }
        public string ContentRootPath { get; }
        public IFileProvider ContentRootFileProvider { get; }
    }
}
