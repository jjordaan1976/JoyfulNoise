using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Tutor.Services;
using Tutor.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Tutor.Web.App>("#app");

// Auth service for token management
builder.Services.AddScoped<IAuthService, AuthService>();

// HTTP client for API calls with authorization
builder.Services.AddScoped<AuthorizingMessageHandler>();
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

AppDomain.CurrentDomain.UnhandledException += (_, e) =>
    Console.Error.WriteLine($"[UnhandledException] {e.ExceptionObject}");

TaskScheduler.UnobservedTaskException += (_, e) =>
{
    Console.Error.WriteLine($"[UnobservedTaskException] {e.Exception}");
    e.SetObserved();
};

var host = builder.Build();

await host.RunAsync();
