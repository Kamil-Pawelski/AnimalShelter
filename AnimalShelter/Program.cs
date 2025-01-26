using AnimalShelter.Constants;
using AnimalShelter.Domain;
using AnimalShelter.Domain.Repositores;
using AnimalShelter.Domain.UserEntities;
using AnimalShelter.Infrastructure.Repositories;
using AnimalShelter.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//AppSetting constants initializer 
AppConfigurationConstants.Initialize(builder.Configuration);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = AppConfigurationConstants.JwtIssuer,
        ValidAudience = AppConfigurationConstants.JwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppConfigurationConstants.JwtSecretKey))
    };
});

builder.Services.AddScoped<JWTService>();

// Mediator
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(AppConfigurationConstants.SqlServerConnectionString);
});

//Logger
Log.Logger = new LoggerConfiguration()
          .MinimumLevel.Debug()
          .WriteTo.Console()
          .CreateLogger();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
