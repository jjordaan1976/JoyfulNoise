using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using MusicSchool.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<MusicSchool.Web.App>("#app");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:64100/")
});

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

// Catch any unhandled exception and write it to the browser console instead of
// crashing the JS debug adapter with exit code 0xffffffff.
AppDomain.CurrentDomain.UnhandledException += (_, e) =>
    Console.Error.WriteLine($"[UnhandledException] {e.ExceptionObject}");

TaskScheduler.UnobservedTaskException += (_, e) =>
{
    Console.Error.WriteLine($"[UnobservedTaskException] {e.Exception}");
    e.SetObserved();
};

await builder.Build().RunAsync();