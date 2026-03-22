using Dapper;
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
            // Register Dapper type handler so TIME columns map to TimeOnly
            SqlMapper.AddTypeHandler(new TimeOnlyTypeHandler());

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
            services.AddScoped<IAccountHolderDataAccessObject, AccountHolderDataAccessObject>();
            services.AddScoped<IBundleQuarterDataAccessObject, BundleQuarterDataAccessObject>();
            services.AddScoped<IExtraLessonDataAccessObject, ExtraLessonDataAccessObject>();
            services.AddScoped<IExtraLessonAggregateDataAccessObject, ExtraLessonAggregateDataAccessObject>();
            services.AddScoped<IInvoiceDataAccessObject, InvoiceDataAccessObject>();
            services.AddScoped<ILessonAggregateDataAccessObject, LessonAggregateDataAccessObject>();
            services.AddScoped<ILessonBundleDataAccessObject, LessonBundleDataAccessObject>();
            services.AddScoped<ILessonDataAccessObject, LessonDataAccessObject>();
            services.AddScoped<ILessonTypeDataAccessObject, LessonTypeDataAccessObject>();
            services.AddScoped<IScheduledSlotDataAccessObject, ScheduledSlotDataAccessObject>();
            services.AddScoped<IStudentDataAccessObject, StudentDataAccessObject>();
            services.AddScoped<ITeacherDataAccessObject, TeacherDataAccessObject>();
            services.AddScoped<ILessonBundleAggregateDataAccessObject, LessonBundleAggregateDataAccessObject>();

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
