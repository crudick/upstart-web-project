using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using Upstart.Api.Endpoints;
using Upstart.Api.Mapping;
using Upstart.Api.Middleware;
using Upstart.Api.Validators;
using Upstart.Application.Interfaces;
using Upstart.Application.Services;
using Upstart.Domain.Interfaces;
using Upstart.Persistence.Data;
using Upstart.Persistence.Repositories;

// Configure Serilog early
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting Upstart API");

    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog for logging - configuration comes from appsettings
    builder.Host.UseSerilog((context, configuration) =>
    {
        configuration.ReadFrom.Configuration(context.Configuration);
    });

    // Add services to the container.
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Add AutoMapper
    builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

    // Add FluentValidation
    builder.Services.AddValidatorsFromAssemblyContaining<CreateUserApiRequestValidator>();

    // Add CORS
    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowReactApp", policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });

    // Add Services
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IPollService, PollService>();
    builder.Services.AddScoped<IPollAnswerService, PollAnswerService>();
    builder.Services.AddScoped<IPollStatService, PollStatService>();
    builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

    // Add Entity Framework
    builder.Services.AddDbContext<UpstartDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("UpstartDb"))
               .UseSnakeCaseNamingConvention());

    // Add repositories
    builder.Services.AddScoped<IUsersRepository, UsersRepository>();
    builder.Services.AddScoped<IPollsRepository, PollsRepository>();
    builder.Services.AddScoped<IPollAnswersRepository, PollAnswersRepository>();
    builder.Services.AddScoped<IPollStatsRepository, PollStatsRepository>();

    // Add JWT Authentication
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings["SecretKey"];
    
    if (string.IsNullOrEmpty(secretKey))
    {
        throw new InvalidOperationException("JWT SecretKey is not configured in appsettings");
    }

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey))
            };
        });

    builder.Services.AddAuthorization();

    // Add CSRF protection
    builder.Services.AddAntiforgery(options =>
    {
        options.HeaderName = "X-CSRF-TOKEN";
        options.Cookie.Name = "__Host-X-CSRF-TOKEN";
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.HttpOnly = true;
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<GlobalExceptionMiddleware>();

    // Add Serilog request logging
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.FirstOrDefault());
            diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress?.ToString());
        };
    });

    app.UseCors("AllowReactApp");

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    // Map endpoints
    app.MapCsrfEndpoints();
    app.MapAuthEndpoints();
    app.MapUsersEndpoints();
    app.MapPollsEndpoints();
    app.MapPollAnswersEndpoints();
    app.MapPollStatsEndpoints();

    Log.Information("Upstart API started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Upstart API terminated unexpectedly");
}
finally
{
    Log.Information("Upstart API is shutting down");
    Log.CloseAndFlush();
}

public partial class Program { }