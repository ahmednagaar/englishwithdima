using System.Text;
using EnglishPlatform.API.Hubs;
using EnglishPlatform.API.Jobs;
using EnglishPlatform.Application.Interfaces;
using EnglishPlatform.Application.Mappings;
using EnglishPlatform.Application.Services;
using EnglishPlatform.Domain.Entities;
using EnglishPlatform.Infrastructure.Data;
using EnglishPlatform.Infrastructure.Repositories.Implementations;
using EnglishPlatform.Infrastructure.Repositories.Interfaces;
using EnglishPlatform.Infrastructure.UnitOfWork;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ===== Serilog =====
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// ===== Database =====
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.MigrationsAssembly("EnglishPlatform.Infrastructure");
            sqlOptions.EnableRetryOnFailure(3);
        }));

// ===== Identity =====
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.User.RequireUniqueEmail = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// ===== JWT Authentication =====
var jwtKey = builder.Configuration["Jwt:SecretKey"] ?? "ThisIsASuperSecretKeyThatIsLongEnoughForHS256!@#2024";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "EnglishWithDima",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "EnglishWithDimaApp",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
})
.AddFacebook(options =>
{
    options.AppId = builder.Configuration["Facebook:AppId"] ?? "";
    options.AppSecret = builder.Configuration["Facebook:AppSecret"] ?? "";
});

// ===== Hangfire =====
builder.Services.AddHangfire(config =>
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
          .UseSimpleAssemblyNameTypeSerializer()
          .UseRecommendedSerializerSettings()
          .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"),
              new SqlServerStorageOptions
              {
                  CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                  SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                  QueuePollInterval = TimeSpan.Zero,
                  UseRecommendedIsolationLevel = true,
                  DisableGlobalLocks = true
              }));
builder.Services.AddHangfireServer();

// ===== AutoMapper =====
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// ===== Repositories & UoW =====
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ITestRepository, TestRepository>();
builder.Services.AddScoped<ILeaderboardRepository, LeaderboardRepository>();
builder.Services.AddScoped<IStudentProgressRepository, StudentProgressRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ===== Application Services =====
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IGradeService, GradeService>();
builder.Services.AddScoped<IQuestionService, QuestionService>();
builder.Services.AddScoped<ITestService, TestService>();
builder.Services.AddScoped<IMatchingGameService, MatchingGameService>();
builder.Services.AddScoped<IWheelGameService, WheelGameService>();
builder.Services.AddScoped<IDragDropGameService, DragDropGameService>();
builder.Services.AddScoped<IFlipCardGameService, FlipCardGameService>();

// ===== Real-Time & Background =====
builder.Services.AddSingleton<INotificationSender, NotificationSender>();
builder.Services.AddSingleton<BackgroundJobs>();

// ===== CORS =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",
                "https://localhost:4200",
                "http://localhost:5173",
                "https://englishwithdima.vercel.app")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// ===== Controllers =====
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "English with Dima API",
        Version = "v1",
        Description = "Educational platform API for English learning"
    });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your token"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddSignalR();

var app = builder.Build();

// ===== Middleware Pipeline =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "English with Dima API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.UseHangfireDashboard("/hangfire");
app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");

// ===== Seed Roles =====
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { "Student", "Parent", "Teacher", "Admin", "SuperAdmin" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // ===== Seed Default Teacher Account =====
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var teacherUser = await userManager.FindByNameAsync("dima");
    if (teacherUser == null)
    {
        teacherUser = new ApplicationUser
        {
            UserName = "dima",
            Email = "dima@englishwithdima.com",
            FirstName = "ديما",
            LastName = "المعلمة",
            Role = "Teacher",
            PreferredLanguage = "ar",
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            EmailConfirmed = true
        };
        var createResult = await userManager.CreateAsync(teacherUser, "Dima@2024");
        if (createResult.Succeeded)
        {
            await userManager.AddToRoleAsync(teacherUser, "Teacher");
            Log.Information("✅ Default teacher account created: dima / Dima@2024");
        }
    }
}

// ===== Schedule Hangfire Recurring Jobs =====
RecurringJob.AddOrUpdate<BackgroundJobs>("refresh-leaderboard",
    j => j.RefreshLeaderboardAsync(), "*/15 * * * *"); // every 15 minutes
RecurringJob.AddOrUpdate<BackgroundJobs>("evaluate-badges",
    j => j.EvaluateBadgesAsync(), "*/30 * * * *"); // every 30 minutes
RecurringJob.AddOrUpdate<BackgroundJobs>("cleanup-guests",
    j => j.CleanupGuestSessionsAsync(), Cron.Daily); // daily
RecurringJob.AddOrUpdate<BackgroundJobs>("expire-test-attempts",
    j => j.ExpireOverdueTestAttemptsAsync(), "*/5 * * * *"); // every 5 minutes

Log.Information("🚀 English with Dima API started successfully");
app.Run();
