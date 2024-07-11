var builder = DistributedApplication.CreateBuilder(args);

var sample = builder.AddProject<Projects.Sample>("sample")
                    .WithArgs("dump");
var diagnostics = builder.AddProject<Projects.Diagnostics>("diagnostics");

builder.Build().Run();
