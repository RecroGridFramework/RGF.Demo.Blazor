using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Recrovit.RecroGridFramework.Client.Blazor;
using RGF.Demo.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.ConfigureServices();
var host = builder.Build();

await host.Services.InitializeRgfBlazorAsync();
//Configure.RegisterEntityComponent();

await host.RunAsync();
