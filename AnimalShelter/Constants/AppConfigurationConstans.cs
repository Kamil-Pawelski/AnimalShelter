using Microsoft.Extensions.Configuration;

namespace AnimalShelter.Constants;

public static class AppConfigurationConstants
{
    public static string? SqlServerConnectionString { get; private set; }
    public static string? JwtAudience { get; private set; }
    public static string? JwtIssuer { get; private set; }
    public static string? JwtSecretKey { get; private set; }
    public static void Initialize(IConfiguration configuration)
    {
        SqlServerConnectionString = configuration.GetConnectionString("DefaultConnection");

        JwtAudience = configuration.GetValue<string>("JWT:Audience");
        JwtIssuer = configuration.GetValue<string>("JWT:Issuer");
        JwtSecretKey = configuration.GetValue<string>("JWT:SecretKey");
    }
}

