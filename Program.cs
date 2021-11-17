using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

((IConfigurationBuilder)builder.Configuration).Add(new MyConfigurationSource());

builder.Services
    .AddOptions()
    .Configure<MyOptions>(builder.Configuration.GetSection("MyOptions"))
    .AddScoped((p) => p.GetRequiredService<IOptionsMonitor<MyOptions>>().CurrentValue);

var app = builder.Build();

app.MapGet("/value", (MyOptions options) => Results.Json(options));
app.MapPost("/reload", (IConfiguration config) => ((IConfigurationRoot)config).Reload());

app.Run();

public class MyOptions
{
    public string Value { get; set; } = "foo";
}

public class MyConfigurationSource : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder) => new MyConfigurationProvider();
}

public class MyConfigurationProvider : ConfigurationProvider
{
    public override void Load() => LoadAsync().GetAwaiter().GetResult();

    private async Task LoadAsync()
    {
        // Simulate a network call to get remote configuration, similar to the Azure Key Vault provider
        // https://github.com/Azure/azure-sdk-for-net/blob/7268b8a22b11df2465f4c7824ad524a9428d681d/sdk/extensions/Azure.Extensions.AspNetCore.Configuration.Secrets/src/AzureKeyVaultConfigurationProvider.cs#L56
        await Task.Delay(150);
    }
}
