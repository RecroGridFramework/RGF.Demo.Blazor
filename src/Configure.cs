using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Recrovit.RecroGridFramework.Client.Blazor;
using Syncfusion.Blazor;

namespace RGF.Demo.Blazor;

public static class Configure
{
    public static WebAssemblyHostBuilder ConfigureServices(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddOidcAuthentication(options =>
        {
            var provider = builder.Configuration.GetValue<string>("Oidc:Provider", "Duende");
            builder.Configuration.Bind($"Oidc:{provider}:ProviderOptions", options.ProviderOptions);
            builder.Configuration.Bind($"Oidc:{provider}:UserOptions", options.UserOptions);
        });

        var loggerFactory = builder.Services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger<Program>();
        builder.Services.AddRgfBlazorServices(builder.Configuration, logger);

#if DevExpressEnabled
        builder.Services.AddDevExpressBlazor(configure => configure.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5);
#endif
        builder.Services.AddScoped<Radzen.DialogService>();

        builder.Services.AddSyncfusionBlazor();
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(builder.Configuration.GetValue<string>("Syncfusion:LicenseKey"));

#if TelerikEnabled
        builder.Services.AddTelerikBlazor();
#endif
        return builder;
    }
}
