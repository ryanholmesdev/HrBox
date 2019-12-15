using HrBoxApi.Data;
using HrBoxApi.Middleware;
using HrBoxApi.Models.Settings;
using HrBoxApi.Services;
using HrBoxApi.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using Hangfire;
using Hangfire.SqlServer;
using HrBoxApi.Jobs;
using HrBoxApi.Jobs.Interfaces;

namespace HrBoxApi
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      // Get the settings from appsettings.json and strongly type it.
      var appSettingsSection = Configuration.GetSection("AppSettings");
      services.Configure<AppSettings>(appSettingsSection);

      AppSettings appSettings = appSettingsSection.Get<AppSettings>();

      var key = Encoding.ASCII.GetBytes(appSettings.JWTSecret);

      // add jwt auth
      services.AddAuthentication(x =>
      {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer(x =>
      {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(key),
          ValidateIssuer = false,
          ValidateAudience = false,
          ValidateLifetime = true,
          ClockSkew = TimeSpan.Zero
        };
      });






      // Add the database context from the default connection string in the appsettings.json
      services.AddDbContext<AppDbContext>(options => options.UseSqlServer(appSettings.DefaultConnection));

      // TODO: Restrict this to just the orgins that are needed.
      // Enable cors for all orgins atm 
      //services.AddCors(options =>
      //{
      //  options.AddPolicy(MyAllowSpecificOrigins,
      //  builder =>
      //  {
      //    builder.WithOrigins("http://localhost:4200/");
      //  });
      //});
      //services.AddControllers();



      services.AddCors(o => o.AddPolicy("MyAllowSpecificOrigins", builder =>
      {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
      }));
      
      // Add Hangfire services.
      services.AddHangfire(configuration => configuration
          .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
          .UseSimpleAssemblyNameTypeSerializer()
          .UseRecommendedSerializerSettings()
          .UseSqlServerStorage(appSettings.DefaultConnection, new SqlServerStorageOptions
          {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            UsePageLocksOnDequeue = true,
            DisableGlobalLocks = true
          }));

      // Add the processing server as IHostedService
      services.AddHangfireServer();

      services.AddControllers();

      // Services...
      services.AddScoped<IUserService, UserService>();
      services.AddScoped<IAuthService, AuthService>();

      // Jobs...
      services.AddScoped<ITokenJob, TokenJob>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseHangfireDashboard();

      // TODO: Update a proper cors policy, Enable cors
      app.UseCors(MyAllowSpecificOrigins);

      app.UseHttpsRedirection();

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseMiddleware(typeof(ErrorHandlingMiddleware));

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });

      // Schedule all background jobs.
      HangfireJobScheduler.ScheduleRecurringJobs();
    }
  }
}
