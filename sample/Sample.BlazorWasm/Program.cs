using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Rocket.Surgery.WebAssembly.Hosting;
using Sample.BlazorWasm;

var builder = WebAssemblyHostBuilder
   .CreateDefault(args);
//                   .ConfigureRocketSurgery(provider => Enumerable.Empty<IConventionMetadata>());
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new(builder.HostEnvironment.BaseAddress), });

var host = await builder.ConfigureRocketSurgery();
await host.RunAsync();
