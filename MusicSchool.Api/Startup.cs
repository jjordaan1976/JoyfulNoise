using Microsoft.Data.SqlClient;
using MusicSchool.Data.Implementations;
using MusicSchool.Data.Interfaces;
using Scalar.AspNetCore;
using Serilog;
using System.Data;

namespace MusicSchool.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Database connection
            services.AddScoped<IDbConnection>(_ =>
                new SqlConnection(Configuration.GetConnectionString("MusicSchool")));

            // Repositories
            services.AddScoped<ITeacherRepository, TeacherRepository>();
            services.AddScoped<IAccountHolderRepository, AccountHolderRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ILessonTypeRepository, LessonTypeRepository>();
            services.AddScoped<ILessonBundleRepository, LessonBundleRepository>();
            services.AddScoped<IScheduledSlotRepository, ScheduledSlotRepository>();
            services.AddScoped<ILessonRepository, LessonRepository>();
            services.AddScoped<IExtraLessonRepository, ExtraLessonRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IAccountHolderService, AccountHolderService>();
            services.AddScoped<IBundleQuarterService, BundleQuarterService>();
            services.AddScoped<IExtraLessonService, ExtraLessonService>();
            services.AddScoped<IExtraLessonAggregateService, ExtraLessonAggregateService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<ILessonAggregateService, LessonAggregateService>();
            services.AddScoped<ILessonBundleAggregateService, LessonBundleAggregateService>();
            services.AddScoped<ILessonBundleService, LessonBundleService>();
            services.AddScoped<ILessonService, LessonService>();
            services.AddScoped<ILessonTypeService, LessonTypeService>();
            services.AddScoped<IScheduledSlotService, ScheduledSlotService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ITeacherService, TeacherService>();

            services.AddControllers();

            services.AddOpenApi();
        }

        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate =
                    "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
            });

            if (env.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference(options =>
                {
                    options.Title = "MusicSchool API";
                    options.Theme = ScalarTheme.DeepSpace;
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
        }
    }
}
