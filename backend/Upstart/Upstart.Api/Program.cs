using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Upstart.Api.Endpoints;
using Upstart.Api.Mapping;
using Upstart.Api.Middleware;
using Upstart.Api.Validators;
using Upstart.Application.Services;
using Upstart.Domain.Interfaces;
using Upstart.Persistence.Data;
using Upstart.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserApiRequestValidator>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add Services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<LoanService>();

// Add Entity Framework
builder.Services.AddDbContext<UpstartDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("UpstartDb"))
           .UseSnakeCaseNamingConvention());

// Add repositories
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<ILoansRepository, LoansRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseCors("AllowReactApp");

app.UseHttpsRedirection();

// Map endpoints
app.MapUsersEndpoints();
app.MapLoansEndpoints();

app.Run();

public partial class Program { }