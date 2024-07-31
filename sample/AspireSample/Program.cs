using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var sample = builder
            .AddProject<Sample>("sample")
            .WithArgs("dump");
var diagnostics = builder.AddProject<Diagnostics>("diagnostics");

builder.Build().Run();