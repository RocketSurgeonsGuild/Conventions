using Microsoft.Extensions.FileProviders;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    ///  IRocketEnvironment
    /// </summary>
    public interface IRocketEnvironment
    {
        /// <summary>
        /// Gets or sets the name of the environment. The host automatically sets this property to the value of the
        /// of the "environment" key as specified in configuration.
        /// </summary>
        /// <value>The name of the environment.</value>
        string EnvironmentName { get; }

        /// <summary>
        /// Gets or sets the name of the application. This property is automatically set by the host to the assembly containing
        /// the application entry point.
        /// </summary>
        /// <value>The name of the application.</value>
        string ApplicationName { get; }

        /// <summary>
        /// Gets or sets the absolute path to the directory that contains the application
        /// content files.
        /// </summary>
        /// <value>The content root path.</value>
        string? ContentRootPath { get; }

        /// <summary>
        /// Gets or sets an Microsoft.Extensions.FileProviders.IFileProvider pointing
        /// at Microsoft.Extensions.Hosting.IHostingEnvironment.ContentRootPath.
        /// </summary>
        /// <value>The content root file provider.</value>
        IFileProvider? ContentRootFileProvider { get; }
    }
}
