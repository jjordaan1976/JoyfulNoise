using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Tutor.StudentPortal;
using Tutor.StudentPortal.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Auth service for token management
builder.Services.AddScoped<IAuthService, AuthService>();

// HTTP client for API calls with authorization
builder.Services.AddScoped<AuthorizingMessageHandler>();
builder.Services.AddHttpClient("API", client =>
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:64100/"))
    .AddHttpMessageHandler<AuthorizingMessageHandler>();

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

builder.Services.AddScoped<ApiService>();
builder.Services.AddMudServices();

AppDomain.CurrentDomain.UnhandledException += (_, e) =>
    Console.Error.WriteLine($"[UnhandledException] {e.ExceptionObject}");

TaskScheduler.UnobservedTaskException += (_, e) =>
{
    Console.Error.WriteLine($"[UnobservedTaskException] {e.Exception}");
    e.SetObserved();
};

await builder.Build().RunAsync();
