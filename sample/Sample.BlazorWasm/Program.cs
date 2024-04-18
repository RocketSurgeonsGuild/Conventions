using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Rocket.Surgery.WebAssembly.Hosting;
using Sample.BlazorWasm;

var builder = await WebAssemblyHostBuilder
                   .CreateDefault(args)
                   .ConfigureRocketSurgery(Imports.Instance);
//                   .ConfigureRocketSurgery(provider => Enumerable.Empty<IConventionWithDependencies>());
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new(builder.HostEnvironment.BaseAddress), });

await builder.Build().RunAsync();