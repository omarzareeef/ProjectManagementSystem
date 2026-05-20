namespace PMS.API.Extensions;

public static class CorsExtensions
{
    public const string PolicyName = "AppCors";

    public static IServiceCollection AddAppCors(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? new[] { "http://localhost:4200" };

        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, policy => policy
                .WithOrigins(origins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .WithExposedHeaders("Content-Disposition"));
        });

        return services;
    }
}
