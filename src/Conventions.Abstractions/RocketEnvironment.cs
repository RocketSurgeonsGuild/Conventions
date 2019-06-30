using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// RocketEnvironment.
    /// Implements the <see cref="Rocket.Surgery.Conventions.IRocketEnvironment" />
    /// </summary>
    /// <seealso cref="Rocket.Surgery.Conventions.IRocketEnvironment" />
    public class RocketEnvironment : IRocketEnvironment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RocketEnvironment"/> class.
        /// </summary>
        /// <param name="environmentName">Name of the environment.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="contentRootPath">The content root path.</param>
        /// <param name="contentRootFileProvider">The content root file provider.</param>
        public RocketEnvironment(string environmentName, string applicationName, string contentRootPath, IFileProvider contentRootFileProvider)
        {
            EnvironmentName = environmentName;
            ApplicationName = applicationName;
            ContentRootPath = contentRootPath;
            ContentRootFileProvider = contentRootFileProvider;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="RocketEnvironment"/> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        public RocketEnvironment(IHostingEnvironment environment)
        {
            EnvironmentName = environment.EnvironmentName;
            ApplicationName = environment.ApplicationName;
            ContentRootPath = environment.ContentRootPath;
            ContentRootFileProvider = environment.ContentRootFileProvider;
        }
#if NETCOREAPP3_0
        /// <summary>
        /// Initializes a new instance of the <see cref="RocketEnvironment"/> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        public RocketEnvironment(IHostEnvironment environment)
        {
            EnvironmentName = environment.EnvironmentName;
            ApplicationName = environment.ApplicationName;
            ContentRootPath = environment.ContentRootPath;
            ContentRootFileProvider = environment.ContentRootFileProvider;
        }
#endif

        /// <summary>
        /// Gets or sets the name of the environment. The host automatically sets this property to the value of the
        /// of the "environment" key as specified in configuration.
        /// </summary>
        /// <value>The name of the environment.</value>
        public string EnvironmentName { get; }
        /// <summary>
        /// Gets or sets the name of the application. This property is automatically set by the host to the assembly containing
        /// the application entry point.
        /// </summary>
        /// <value>The name of the application.</value>
        public string ApplicationName { get; }
        /// <summary>
        /// Gets or sets the absolute path to the directory that contains the application
        /// content files.
        /// </summary>
        /// <value>The content root path.</value>
        public string ContentRootPath { get; }
        /// <summary>
        /// Gets or sets an Microsoft.Extensions.FileProviders.IFileProvider pointing
        /// at Microsoft.Extensions.Hosting.IHostingEnvironment.ContentRootPath.
        /// </summary>
        /// <value>The content root file provider.</value>
        public IFileProvider ContentRootFileProvider { get; }
    }
}
