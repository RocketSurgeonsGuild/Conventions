using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.CommandLine;
using Sample.Core;
using System;

namespace Sample
{
    public class DefaultCommand : IDefaultCommand
    {
        private readonly IService _service;

        public DefaultCommand(IService service) => _service = service;

        public int Run(IApplicationState state)
        {
            Console.WriteLine(_service.GetString());
            return 1;
        }
    }
}