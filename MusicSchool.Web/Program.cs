using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using MusicSchool.Auth;
using MusicSchool.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<MusicSchool.Web.App>("#app");

// Auth
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, GoogleAuthStateProvider>();
builder.Services.AddAuthorizationCore(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// HTTP client with auth handler — all services receive this instance
builder.Services.AddTransient<AuthorizingMessageHandler>();
builder.Services.AddHttpClient("API", client =>
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:64100/"))
    .AddHttpMessageHandler<AuthorizingMessageHandler>();

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

builder.Services.AddMudServices();

builder.Services.AddScoped<TeacherService>();
builder.Services.AddScoped<AccountHolderService>();
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<LessonTypeService>();
builder.Services.AddScoped<LessonBundleService>();
builder.Services.AddScoped<ScheduledSlotService>();
builder.Services.AddScoped<LessonService>();
builder.Services.AddScoped<ExtraLessonService>();
builder.Services.AddScoped<InvoiceService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<MagicLinkService>();

AppDomain.CurrentDomain.UnhandledException += (_, e) =>
    Console.Error.WriteLine($"[UnhandledException] {e.ExceptionObject}");

TaskScheduler.UnobservedTaskException += (_, e) =>
{
    Console.Error.WriteLine($"[UnobservedTaskException] {e.Exception}");
    e.SetObserved();
};

var host = builder.Build();

// Restore session from sessionStorage before first render
var authService = host.Services.GetRequiredService<AuthService>();
await authService.InitializeAsync();

await host.RunAsync();
