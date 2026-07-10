using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Tutor.Data.Implementations;
using Tutor.Data.Interfaces;
using Tutor.JwtService;
using Tutor.Repositories;
using Scalar.AspNetCore;
using Serilog;
using System.Data;
using System.Text;

namespace Tutor.Api
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
                new SqlConnection(Configuration.GetConnectionString("Tutor")));

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
            services.AddScoped<IScheduledSlotAggregateDataAccessObject, ScheduledSlotAggregateDataAccessObject>();
            services.AddScoped<IPaymentDataAccessObject, PaymentDataAccessObject>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IUserDataAccessObject, UserDataAccessObject>();
            services.AddScoped<IUserRepository, UserRepository>();

            // Email service — mock console sender for this phase of development
            services.AddScoped<IEmailService, ConsoleEmailService>();

            // JWT and OTP services
            var jwtSecret = Configuration["Jwt:Secret"] ?? "your-secret-key-that-is-long-enough-for-hmacsha256-algorithm";
            var jwtIssuer = Configuration["Jwt:Issuer"] ?? "TutorApp";
            var jwtAudience = Configuration["Jwt:Audience"] ?? "TutorUsers";

            services.AddSingleton<IJwtTokenService>(new JwtTokenService(jwtSecret, jwtIssuer, jwtAudience));
            services.AddSingleton<IOtpService, OtpService>();

            // JWT Authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                        ValidateIssuer = true,
                        ValidIssuer = jwtIssuer,
                        ValidateAudience = true,
                        ValidAudience = jwtAudience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Log.Warning(context.Exception, "JWT authentication failed");
                            return Task.CompletedTask;
                        },
                        OnMessageReceived = context =>
                        {
                            Log.Information("JWT token received: {HasToken}", !string.IsNullOrEmpty(context.Token));
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization();
            services.AddControllers();
            services.AddOpenApi();
        }

        public void Configure(WebApplication app, IWebHostEnvironment env, string corsPolicyName)
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
                    options.Title = "Tutor API";
                    options.Theme = ScalarTheme.DeepSpace;
                });
            }

            app.UseHttpsRedirection();
            app.UseCors(corsPolicyName);
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
        }
    }
}
