using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Rocket.Surgery.Hosting.Internals
{
    class HostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = null!;
        public string ApplicationName { get; set; } = null!;
        public string ContentRootPath { get; set; } = null!;
        public IFileProvider ContentRootFileProvider { get; set; } = null!;
    }
}