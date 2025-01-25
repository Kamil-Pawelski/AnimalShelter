namespace AnimalShelter.Constants;

public static class AppConfigurationConstants
{
    public static string? SqlServerConnectionString { get; private set; }
    public static void Initialize(IConfiguration configuration)
    {
        SqlServerConnectionString = configuration.GetConnectionString("DefaultConnection");
    }
}

